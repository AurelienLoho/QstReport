/**********************************************************************************************/
/**** Fichier : RawEpeiresEvent.cs                                                         ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Epeires
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Les données brutes d'un évèvenement notifié dans EPEIRES.
    /// </summary>
    [DataContract]
    public sealed class RawEpeiresEvent : IEquatable<RawEpeiresEvent>
    {
        /// <summary>
        /// L'identifiant.
        /// </summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Le nom.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Indique si l'évènement est modifiable.
        /// </summary>
        [DataMember(Name = "modifiable")]
        public bool Modifiable { get; set; }

        /// <summary>
        /// La date de début.
        /// </summary>
        [DataMember(Name = "start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// La date de fin.
        /// </summary>
        [DataMember(Name = "end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Indique si l'évènement est ponctuel.
        /// </summary>
        [DataMember(Name = "ponctual")]
        public bool Ponctual { get; set; }

        /// <summary>
        /// La catégorie racine.
        /// </summary>
        [DataMember(Name = "category_root")]
        public string CategoryRoot { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "category_root_id")]
        public int CategoryRootId { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "category_root_short")]
        public string CategoryRootShort { get; set; }

        /// <summary>
        /// La catégorie.
        /// </summary>
        [DataMember(Name = "category")]
        public string Category { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "category_id")]
        public int CategoryId { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "category_short")]
        public string CategoryShort { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "category_compact")]
        public bool CategoryCompact { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "category_place")]
        public int CategoryPlace { get; set; }

        /// <summary>
        /// Le statut.
        /// </summary>
        [DataMember(Name = "status_name")]
        public string StatusName { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "status_id")]
        public int StatusId { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "impact_value")]
        public int ImpactValue { get; set; }

        /// <summary>
        /// L'impact.
        /// </summary>
        [DataMember(Name = "impact_name")]
        public string ImpactName { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "impact_style")]
        public string ImpactStyle { get; set; }

        /// <summary>
        /// Le nombre de fichiers.
        /// </summary>
        [DataMember(Name = "files")]
        public int Files { get; set; }

        /// <summary>
        /// L'URL du premier fichier.
        /// </summary>
        [DataMember(Name = "url_file1")]
        public string UrlFile1 { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [DataMember(Name = "star")]
        public bool Star { get; set; }

        /// <summary>
        /// Indique si l'évènement est programmé.
        /// </summary>
        [DataMember(Name = "scheduled")]
        public bool Scheduled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "fields")]
        public Dictionary<string, string> Fields { get; set; }

        /// <summary>
        /// Le titre de l'évènement.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RawEpeiresEvent other)
        {
            if (object.ReferenceEquals(other, null)) { return false; }

            return this.Id == other.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as RawEpeiresEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(RawEpeiresEvent left, RawEpeiresEvent right)
        {
            if (object.ReferenceEquals(left, right)) { return true; }
            if (object.ReferenceEquals(left, null)) { return false; }

            return left.Equals(right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(RawEpeiresEvent left, RawEpeiresEvent right)
        {
            return !(left == right);
        }
    }
}
