/**********************************************************************************************/
/**** Fichier : SiamParser.cs                                                              ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam
{
    using HtmlAgilityPack;
    using QstReport.DataModel;
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class SiamParser
    {
        /// <summary>
        /// Parse une balise HTML et récupère les informations concernant un AVT.
        /// </summary>
        /// <param name="node">La balise HTML contenant les informations</param>
        /// <returns>Les données concernant un AVT.</returns>
        public static Avt ParseHtmlAsAvt(HtmlNode node)
        {
            var refSiamRaw = node.SelectSingleNode("//h1[@class='bandeau']").InnerText;
            var refSiam = refSiamRaw.Replace("Avis Travaux", string.Empty).TrimStart();

            var rawTitle = node.SelectSingleNode("//div[@class='w900']/h2").InnerText;
            var cutIndex = rawTitle.IndexOf(']');

            var refAvt = rawTitle.Substring(1, cutIndex - 1);
            var title = rawTitle.Substring(cutIndex + 1);

            var infoNodes = node.SelectNodes("//table[@class='form']/tbody/tr").ToList();

            if (infoNodes[1].InnerText.StartsWith("Début"))
            {
                var startColumns = infoNodes[1].SelectNodes("td").ToList();
                // HACK : cannot parse date with 1er
                var startDate = DateTime.ParseExact(startColumns[1].InnerText.Replace("1er", "1"), "dddd d MMMM yyyy à HH:mm UTC", new CultureInfo("fr-Fr"));

                var endColumns = infoNodes[2].SelectNodes("td").ToList();
                // HACK : cannot parse date with 1er
                var endDate = DateTime.ParseExact(endColumns[1].InnerText.Replace("1er", "1"), "dddd d MMMM yyyy à HH:mm UTC", new CultureInfo("fr-FR"));

                var avt = new Avt()
                {
                    Title = title,
                    Id = refAvt,
                    RefSiam = refSiam,
                    WorkPeriods = new TimePeriodCollection { new TimePeriod(startDate, endDate) },
                    Pole = PoleFromRefSiam(refSiam),
                    Entite = EntiteFromRefSiam(refSiam),
                    Description = HtmlEntity.DeEntitize(infoNodes[8].InnerText),
                    Consequences = HtmlEntity.DeEntitize(infoNodes[10].SelectNodes("td")[1].InnerText),
                    ImpactedEquipments = HtmlEntity.DeEntitize(infoNodes[5].SelectNodes("td")[1].InnerText).Split(',').ToList(),
                    ImpactedAirports = HtmlEntity.DeEntitize(infoNodes[15].SelectNodes("td")[1].InnerText).Split(',').Select(x => ConvertToAirportCode(x)).ToList(),
                };

                return avt;
            }
            else
            {
                var periodNodes = infoNodes[1].SelectNodes(".//tbody/tr").ToList();


                var timePeriodCollection = new TimePeriodCollection();

                foreach (var p in periodNodes)
                {
                    var columns = p.SelectNodes("td").ToList();

                    var startDate = DateTime.ParseExact(columns[0].InnerText.Substring(4), "g", new CultureInfo("fr-FR"));
                    var endDate = DateTime.ParseExact(columns[2].InnerText.Substring(4), "g", new CultureInfo("fr-FR"));

                    timePeriodCollection.Add(new TimePeriod(startDate, endDate));
                }

                var avt = new Avt()
                {
                    Title = title,
                    Id = refAvt,
                    RefSiam = refSiam,
                    WorkPeriods = timePeriodCollection,
                    Pole = PoleFromRefSiam(refSiam),
                    Entite = EntiteFromRefSiam(refSiam),
                    Description = HtmlEntity.DeEntitize(infoNodes[7].InnerText),
                    Consequences = HtmlEntity.DeEntitize(infoNodes[9].SelectNodes("td")[1].InnerText),
                    ImpactedEquipments = HtmlEntity.DeEntitize(infoNodes[4].SelectNodes("td")[1].InnerText).Split(',').ToList(),
                    ImpactedAirports = HtmlEntity.DeEntitize(infoNodes[14].SelectNodes("td")[1].InnerText).Split(',').Select(x => ConvertToAirportCode(x)).ToList(),
                };

                return avt;
            }
        }

        /// <summary>
        /// Parse une balise HTML et récupère les identifiants de tous les AVT qu'elle contient.
        /// </summary>
        /// <param name="node">La balise HTML contenant les informations.</param>
        /// <returns>Une collection d'identifiants des AVT.</returns>
        public static IEnumerable<Tuple<int, string>> GetAvtIds(HtmlNode node)
        {
            var avtNodes = node.SelectNodes("//table[@class='tblContext']/tbody/tr");

            return avtNodes.Select(x => ParseHtmlNodeAsAvtId(x));
        }

        /// <summary>
        /// Parse une balise HTML et récupère la référence SIAM et l'identifiant ST de l'AVT.
        /// </summary>
        /// <param name="node">La balise HTML contenant les informations.</param>
        /// <returns>Un couple contenant la référence SIAM et l'identifiant de l'AVT.</returns>
        private static Tuple<int, string> ParseHtmlNodeAsAvtId(HtmlNode node)
        {
            var idText = node.GetAttributeValue("id", string.Empty);
            var caretIndex = idText.LastIndexOf('-');
            var result = Int32.TryParse(idText.Substring(caretIndex + 1), out int id);
            if (!result)
            {
                return null;
            }

            // Les titres des AVT débutent par [XXX/YYY] où : 
            // XXX => pôle rédacteur de l'AVT
            // YYY => identifiant de l'AVT
            var title = node.SelectSingleNode(".//a").InnerText;
            var cutIndex = title.IndexOf(']');
            if (cutIndex < 1)
            {
                return null;
            }

            return new Tuple<int, string>(id, title.Substring(1, cutIndex - 1));
        }

        /// <summary>
        /// Retourne le code OACI de l'aéroport à partir de la dénomination SIAM.
        /// </summary>
        /// <param name="siamLocation">Le nom du site SIAM</param>
        /// <returns></returns>
        private static string ConvertToAirportCode(string siamLocation)
        {
            switch (siamLocation)
            {
                case "Aéroport Le Bourget": { return "LFPB"; }
                case "Aéroport Roissy/CDG": { return "LFPG"; }
                default: { return siamLocation; }
            }
        }

        /// <summary>
        /// Obtient le nom de la subdivision à partir de la référence SIAM.
        /// </summary>
        /// <param name="refSiam">Une référence SIAM.</param>
        /// <returns>Le nom de la Subdivision.</returns>
        private static Entite EntiteFromRefSiam(string refSiam)
        {
            //
            // Les références d'AVT SIAM sont au format : TVX-XXX-AA-iii
            // avec :
            //      XXX = trigramme de la section à l'origine de l'AVT
            //      AA  = Année de l'avt
            //      iii = index
            //
            var section = refSiam.Substring(4, 3).ToLower();

            switch (section)
            {
                case "qst": return Entite.QualiteDeService;
                case "rnv": return Entite.Radionavigation;
                //case "rad": 
                case "sol": return Entite.Radars;
                case "app": return Entite.Radars;
                case "sur": return Entite.Radars;
                //case "cau":
                case "str": return Entite.TraitementRadar;
                case "tpv": return Entite.TraitementPlanDeVol;
                case "sim": return Entite.SimulationSupervision;
                //case "tel": 
                case "rsx": return Entite.Reseaux;
                case "rdo": // section radio renommée en section rte le 20/07/2016
                case "rte": return Entite.RadioTelephone;
                case "ene": return Entite.EnergieClimatisation;
                default: return Entite.QualiteDeService;
            }
        }

        /// <summary>
        /// Obtient le nom de la subdivision à partir de la référence SIAM.
        /// </summary>
        /// <param name="refSiam">Une référence SIAM.</param>
        /// <returns>Le nom de la Subdivision.</returns>
        private static Pole PoleFromRefSiam(string refSiam)
        {
            //
            // Les références d'AVT SIAM sont au format : TVX-XXX-AA-iii
            // avec :
            //      XXX = trigramme de la section à l'origine de l'AVT
            //      AA  = Année de l'avt
            //      iii = index
            //
            var section = refSiam.Substring(4, 3).ToLower();

            switch (section)
            {
                case "qst": return Pole.TSV;
                case "rnv":
                //case "rad":
                case "sol":
                //case "tel":
                case "rdo": // section radio renommée en section rte le 20/07/2016
                case "rte":
                case "ene":
                case "sur": return Pole.CNS;
                //case "cau":
                case "app":
                case "str":
                case "tpv":
                case "rsx":
                case "sim": return Pole.ATM;
                default: return Pole.TSV;
            }
        }
    }
}

