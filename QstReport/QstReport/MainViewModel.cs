﻿/**********************************************************************************************/
/**** Fichier : MainViewModel.cs                                                           ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport
{
    using Microsoft.Win32;
    using QstReport.DataModel;
    using QstReport.Epeires;
    using QstReport.Report;
    using QstReport.Siam;
    using QstReport.Utils;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Le view model principal de l'application.
    /// </summary>
    public sealed class MainViewModel : IDisposable, INotifyPropertyChanged
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";

        /// <summary>
        /// Ordonnanceur de tâches.
        /// </summary>
        private BackgroundWorker _worker;
        
        private bool _freeReportMode = false;
        private bool _isRcoReport = true;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ViewModel"/>.
        /// </summary>
        public MainViewModel()
        {
            _worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = false };
            _worker.DoWork += DoWork;
            _worker.ProgressChanged += OnProgressChanged;

            CreateReportCommand = new RelayCommand(_ => _worker.RunWorkerAsync(), _ => !_worker.IsBusy);
            SetCurrentRcoPeriodCommand = new RelayCommand(_ => ComputeCurrentRcoPeriod());
            SetCurrentGsstPeriodCommand = new RelayCommand(_ => ComputeCurrentGsstPeriod());
            SelectFreePeriodCommand = new RelayCommand(_ => SelectFreePeriod());

            ComputeCurrentRcoPeriod();
        }

        private void ComputeCurrentGsstPeriod()
        {
            _freeReportMode = false;
            _isRcoReport = false;
            var currentWeek = new Week(DateTime.Now);

            StartReportPeriod = currentWeek.PreviousWeek().PreviousWeek().Start;
            EndReportPeriod = currentWeek.End;
            ReportFileName = string.Format("{0} Réunion GSST.xls", currentWeek.Start.ToString(DATE_FORMAT));
        }

        private void ComputeCurrentRcoPeriod()
        {
            IsInFreeReportMode = false;
            _isRcoReport = true;
            var currentWeek = new Week(DateTime.Now);

            StartReportPeriod = currentWeek.PreviousWeek().Start;
            EndReportPeriod = currentWeek.End;
            ReportFileName = string.Format("{0} Réunion RCO.xls", currentWeek.Start.ToString(DATE_FORMAT));
        }

        private void SelectFreePeriod()
        {
            IsInFreeReportMode = true;
            OnDateValueChanged();
        }

        private void OnDateValueChanged()
        {
            if (IsInFreeReportMode)
            {
                ReportFileName = string.Format("Bilan du {0} au {1}.xls", _startReportPeriod.ToString(DATE_FORMAT), _endReportPeriod.ToString(DATE_FORMAT));
            }
        }

        /// <summary>
        /// Récupération des données, mises en forme et impression des rapports.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DoWork(object sender, DoWorkEventArgs e)
        {
            //var currentWeek = new Week(DateTime.Now);
            var currentDataPeriod = GetCurrentPeriod();
            var pastDataPeriod = GetPastPeriod();

            var reportData = new ReportData(currentDataPeriod, pastDataPeriod);

            _worker.ReportProgress(10, "Connexion à SIAM...");
            
            using (var siamRepository = new SiamRepository(Properties.Settings.Default.SIAM_HostName,
                                                           Properties.Settings.Default.SIAM_UserName,
                                                           Properties.Settings.Default.SIAM_Password))
            {
                _worker.ReportProgress(20, "Récupération des AVT...");
                reportData.AvtCollection = siamRepository.GetAvts(StartReportPeriod, EndReportPeriod);

                _worker.ReportProgress(20, "Récupération des évènements techniques...");
                reportData.TechEventCollection = siamRepository.GetTechEvents(pastDataPeriod.Start, pastDataPeriod.End);

                _worker.ReportProgress(30, "Déconnexion de SIAM...");
            }

            _worker.ReportProgress(40, "Connexion à EPEIRES...");
            using(var epeiresRepository = new EpeiresRepository(Properties.Settings.Default.EPEIRES_HostName,
                                                                Properties.Settings.Default.EPEIRES_UserName,
                                                                Properties.Settings.Default.EPEIRES_Password))
            {
                _worker.ReportProgress(50, "Récupération des évènements exploitation...");
                reportData.ExploitEventCollection = epeiresRepository.GetExploitEvents(pastDataPeriod.Start, pastDataPeriod.End);

                _worker.ReportProgress(60, "Déconnexion de EPEIRES...");
            }

            _worker.ReportProgress(80, "Mise en forme des données...");


            var selectedFileName = OpenSaveDialog();

            if (string.IsNullOrEmpty(selectedFileName))
            {
                _worker.ReportProgress(100, "Opération annulée par l'utilisateur...");
            }
            else
            {
                _worker.ReportProgress(90, "Sauvegarde du rapport...");                        

                var reportWriter = new ExcelReportWriter();

                if (IsInFreeReportMode)
                {
                    reportWriter.WriteReport(reportData, selectedFileName);
                }
                else
                {
                    var modelFile = _isRcoReport ? Properties.Settings.Default.RCO_Model_File : Properties.Settings.Default.GSST_Model_File;
                    reportWriter.WriteReport(reportData, selectedFileName, modelFile);
                }

                _worker.ReportProgress(100, "Ouverture du rapport");
                Task.Run(() => Process.Start(selectedFileName));
            }
        }
        
        /// <summary>
        /// Met à jour l'interface utilisateur en fonction de l'avancement des travaux.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
            CurrentProgressText = e.UserState.ToString();
        }

        /// <summary>
        /// La commande pour débuter la création du rapport.
        /// </summary>
        public ICommand CreateReportCommand { get; private set; }

        public ICommand SetCurrentRcoPeriodCommand { get; private set; }

        public ICommand SetCurrentGsstPeriodCommand { get; private set; }

        public ICommand SelectFreePeriodCommand { get; private set; }

        /// <summary>
        /// La progression de la tâche.
        /// </summary>
        private double _currentProgress;
        public double CurrentProgress
        {
            get { return _currentProgress; }
            set { SetProperty(ref _currentProgress, value); }
        }

        /// <summary>
        /// La version texte de la progression de la tâche.
        /// </summary>
        private string _currentProgressText;
        public string CurrentProgressText
        {
            get { return _currentProgressText; }
            set { SetProperty(ref _currentProgressText, value); }
        }

        private DateTime _startReportPeriod;
        public DateTime StartReportPeriod
        {
            get { return _startReportPeriod; }
            set 
            {
                if(SetProperty(ref _startReportPeriod, value))
                {
                    OnDateValueChanged();
                }
            }
        }

        private DateTime _endReportPeriod;
        public DateTime EndReportPeriod
        {
            get { return _endReportPeriod; }
            set 
            { 
                if(SetProperty(ref _endReportPeriod, value))
                {
                    OnDateValueChanged();
                }
            }
        }

        private string _reportFileName;
        public string ReportFileName
        {
            get { return _reportFileName; }
            set { SetProperty(ref _reportFileName, value); }
        }

        public bool IsInFreeReportMode
        {
            get { return _freeReportMode; }
            set { SetProperty(ref _freeReportMode, value); }
        }

        private TimePeriod GetCurrentPeriod()
        {
            return new Week(EndReportPeriod);
        }

        private TimePeriod GetPastPeriod()
        {
            var currentPeriod = GetCurrentPeriod();

            return new TimePeriod(StartReportPeriod, currentPeriod.Start.AddDays(-1).Date);
        }

        private string OpenSaveDialog()
        {
            var sfd = new SaveFileDialog();

            sfd.InitialDirectory = Properties.Settings.Default.DefaultSavePath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sfd.FileName = ReportFileName;
            sfd.CheckFileExists = false;

            var result = sfd.ShowDialog();

            if(result.HasValue && result.Value)
            {
                return sfd.FileName;
            }

            return string.Empty;
        }
        
        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Déclenche l'évènement <see cref="RaisePropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Le nom de de la propriété modifiée.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Evènement déclenché lorsque la valeur d'une propriété de l'objet change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Méthode utilitaire pour implémenter l'interface <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">Le type de données du champ à modifier.</typeparam>
        /// <param name="storage">Le champ à modifier.</param>
        /// <param name="value">La nouvelle valeur du champ.</param>
        /// <param name="propertyName">Le nom de la propriété associé au champ.</param>
        /// <returns>true si le champ a été modifié, false autrement.</returns>
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }            

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _worker.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~ViewModel() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
