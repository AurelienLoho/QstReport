/**********************************************************************************************/
/**** Fichier : Siam5/Parser.cs                                                            ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam5
{
    using HtmlAgilityPack;
    using QstReport.DataModel;
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;

    public static class Parser
    {
        /// <summary>
        /// Parse une balise HTML et récupère les informations concernant un AVT.
        /// </summary>
        /// <param name="node">La balise HTML contenant les informations</param>
        /// <returns>Les données concernant un AVT.</returns>
        public static Avt ParseHtmlAsAvt(HtmlNode documentNode)
        {
            var dataNode = documentNode.SelectSingleNode("//div[@id='contentPopup']");
            var avt = new Avt();

            var rawTitle = HtmlEntity.DeEntitize(dataNode.SelectSingleNode("//h2[@class='alignCenter']").InnerText.Trim());

            var splitIndex = rawTitle.IndexOf(']');

            if(splitIndex == -1)
            {
                return null;
            }

            avt.Id = rawTitle.Substring(1, splitIndex - 1);
            avt.Title = rawTitle.Substring(splitIndex + 1).TrimStart();

            avt.WorkPeriods = ParseWorkPeriods(dataNode);
            avt.ImpactedAirports = ParseLocation(dataNode);
            avt.Description = GetDescription(dataNode);
            avt.Consequences = GetConsequences(dataNode);
            avt.Entite = GetOriginEntity(dataNode);
            avt.Pole = PoleFromEntity(avt.Entite);
            avt.AvtType = HasMiso(dataNode) ? AvtType.MISO : AvtType.AVT;
            avt.IsCancelled = GetCancelledStatus(dataNode);

            return avt;
        }

        private static string GetDescription(HtmlNode dataNode)
        {
            var text = dataNode.SelectSingleNode("//div[@id='occ']//legend[contains(text(),'Description')]/following::div").InnerText;

            return HtmlEntity.DeEntitize(text);
        }

        private static string GetConsequences(HtmlNode dataNode)
        {
            var text = dataNode.SelectSingleNode("//div[@id='occ']//legend[contains(text(),'Analyse')]/following::div").InnerText;

            return HtmlEntity.DeEntitize(text);
        }

        private static bool GetCancelledStatus(HtmlNode dataNode)
        {
            return dataNode.SelectSingleNode("//p[@class='status3']") != null;
        }

        private static List<string> ParseLocation(HtmlNode dataNode)
        {            
            var locations = new List<string>();

            var locatioNodes = dataNode.SelectNodes("//div[@id='occ']//label[contains(text(),'Sites')]/following-sibling::label[contains(text(),'Cha')][1]/preceding-sibling::div[preceding-sibling::label[contains(text(),'Sites')]]");

            foreach (var node in locatioNodes)
            {
                locations.Add(node.InnerText);
            }

            return locations;
        }

        private static Entite GetOriginEntity(HtmlNode dataNode)
        {
            var entityNodes = dataNode.SelectNodes("//div[@id='occ']//label[contains(text(),'Enti')]/following-sibling::label[contains(text(),'Super')][1]/preceding-sibling::div[preceding-sibling::label[contains(text(),'Enti')]]");

            foreach (var node in entityNodes)
            {
                var unfilteredList = node.InnerText.Split(',');

                foreach(var item in unfilteredList)
                {
                    var trimmed = HtmlEntity.DeEntitize(item.Trim());

                    if (trimmed.StartsWith("Super")) { continue; }

                    return EntityFromText(trimmed);
                }
            }

            return Entite.Unknown;
        }

        private static TimePeriodCollection ParseWorkPeriods(HtmlNode dataNode)
        {
            var workPeriods = new TimePeriodCollection();

            var entityNodes = dataNode.SelectNodes("//div[@id='occ']//legend[contains(text(),'Occu')]/following-sibling::div[1]/div");

            if (entityNodes != null && entityNodes.Count > 0)
            {
                foreach (var node in entityNodes)
                {
                    var text = HtmlEntity.DeEntitize(node.InnerText).Trim();

                    var start = text.Substring(0, 16);
                    var end = text.Substring(16);

                    var period = new TimePeriod(DateTime.Parse(start), DateTime.Parse(end));

                    workPeriods.Add(period);
                }
            }
            else
            {
                var singlePeriodNode = dataNode.SelectSingleNode("//div[@id='occ']//legend[contains(text(),'Occu')]/following::div[1]");

                var text = HtmlEntity.DeEntitize(singlePeriodNode.InnerText).Trim();

                var start = text.Substring(0, 16);
                var end = text.Substring(16,16);

                var period = new TimePeriod(DateTime.Parse(start), DateTime.Parse(end));

                workPeriods.Add(period);
            }

            return workPeriods;
        }

        private static bool HasMiso(HtmlNode node)
        {
            var misoNode = node.SelectSingleNode("//div[@class='attachment']//a[contains(text(),'MISO')]");

            return misoNode != null;
        }

        private static Pole PoleFromEntityText(List<string> entities)
        {
            return Pole.ATM;
        }

        private static Entite EntityFromText(string entityName)
        {
            switch (entityName.ToLower())
            {
                case "traitement radar": { return Entite.TraitementRadar; }
                case "radionavigation": { return Entite.Radionavigation; }
                case "installations": { return Entite.Installations; }
                case "qst do": { return Entite.QualiteDeService; }
                case "simulation et supervision": { return Entite.SimulationSupervision; }
                case "traitement plans de vols": { return Entite.TraitementPlanDeVol; }
                case "energie et climatisation": { return Entite.EnergieClimatisation; }
                case "radars": { return Entite.Radars; }
                case "radio tã©lã©phone":   // FIX : problème d'encoding du nom de l'entité
                case "radio téléphone": { return Entite.RadioTelephone; }
                case "rã©seaux": // FIX : problème d'encoding du nom de l'entité
                case "réseaux": { return Entite.Reseaux; }
                case "instruction": { return Entite.Instruction; }
                case "informatique bureautique": { return Entite.Bureautique; }
                default: {
                    return Entite.Unknown; 
                }
            }
        }

        private static Pole PoleFromEntity(Entite entity)
        {
            switch (entity)
            {
                case Entite.Bureautique:
                case Entite.QualiteDeService:
                case Entite.Instruction: { return Pole.TSV; }

                case Entite.EnergieClimatisation:
                case Entite.Radars:
                case Entite.Radionavigation:
                case Entite.RadioTelephone:
                case Entite.Installations: { return Pole.CNS; }

                case Entite.SimulationSupervision:
                case Entite.TraitementPlanDeVol:
                case Entite.TraitementRadar: { return Pole.ATM; }

                case Entite.Unknown:
                default: { return Pole.Unknown; }
            }
        }
    }
}
