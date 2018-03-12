/**********************************************************************************************/
/**** Fichier : MainViewModel.cs                                                           ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using QstReport.Utils;
    using QstReport.DataModel;
    using QstReport.Siam;
    using QstReport.Report;
    using System.Threading.Tasks;
    using System.IO;
    using System.Diagnostics;
    using QstReport.Epeires;

    /// <summary>
    /// Le view model principal de l'application.
    /// </summary>
    public sealed class MainViewModel : IDisposable, INotifyPropertyChanged
    {
        private const string SIAM_URL = "siam.tech.cana.ri";
        private const string SIAM_USER = "QST";
        private const string SIAM_PASSWORD = "qstdo";

        private const string EPEIRES_URL = "epeires.cdg-lb.aviation";
        private const string EPEIRES_USER = "QST";
        private const string EPEIRES_PASSWORD = "EpeiresQST";

        /// <summary>
        /// Ordonnanceur de tâches.
        /// </summary>
        private BackgroundWorker _worker;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ViewModel"/>.
        /// </summary>
        public MainViewModel()
        {
            _worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = false };
            _worker.DoWork += DoWork;
            _worker.ProgressChanged += OnProgressChanged;

            CreateReportCommand = new RelayCommand(_ => _worker.RunWorkerAsync(), _ => !_worker.IsBusy);
        }

        /// <summary>
        /// Récupération des données, mises en forme et impression des rapports.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DoWork(object sender, DoWorkEventArgs e)
        {
            var currentWeek = new Week(DateTime.Now);
            var lastWeek = currentWeek.PreviousWeek();

            var reportData = new ReportData { ReportPeriod = new TimePeriod(lastWeek.Start, currentWeek.End) };

            _worker.ReportProgress(10, "Connexion à SIAM...");

            using (var siamRepository = new SiamRepository(SIAM_URL, SIAM_USER, SIAM_PASSWORD))
            {
                _worker.ReportProgress(20, "Récupération des AVT...");
                reportData.AvtCollection = siamRepository.GetAvts(reportData.ReportPeriod.Start, reportData.ReportPeriod.End);

                _worker.ReportProgress(20, "Récupération des évènements techniques...");
                reportData.TechEventCollection = siamRepository.GetTechEvents(lastWeek.Start, lastWeek.End);

                _worker.ReportProgress(30, "Déconnexion de SIAM...");
            }

            _worker.ReportProgress(40, "Connexion à EPEIRES...");
            using(var epeiresRepository = new EpeiresRepository(EPEIRES_URL, EPEIRES_USER, EPEIRES_PASSWORD))
            {
                _worker.ReportProgress(50, "Récupération des évènements exploitation...");
                reportData.ExploitEventCollection = epeiresRepository.GetExploitEvents(lastWeek.Start, lastWeek.End);

                _worker.ReportProgress(60, "Déconnexion de EPEIRES...");
            }

            _worker.ReportProgress(80, "Mise en forme des données...");

            string reportFileName = string.Format("Bilan RCO du {0}.xlsx", currentWeek.Start.ToString("yyyy-MM-dd"));
            string reportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), reportFileName);

            _worker.ReportProgress(90, "Sauvegarde du rapport...");
            var reportWriter = new ExcelReportWriter();
            reportWriter.WriteReport(reportData, reportPath);
            
            _worker.ReportProgress(100, "Ouverture du rapport");
            Task.Run(() => Process.Start(reportPath));
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

        /// <summary>
        /// La progression de la tâche.
        /// </summary>
        private double currentProgress;
        public double CurrentProgress
        {
            get { return currentProgress; }
            set { SetProperty(ref currentProgress, value); }
        }

        /// <summary>
        /// La version texte de la progression de la tâche.
        /// </summary>
        private string currentProgressText;
        public string CurrentProgressText
        {
            get { return currentProgressText; }
            set { SetProperty(ref currentProgressText, value); }
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
