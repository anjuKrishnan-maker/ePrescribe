using System;
using System.Data;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.Impact;
using eRxWeb;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test
{
    [TestClass]
    public class medispanDURTests
    {
        [TestMethod]
        public void PopulateResponseDataShouldPopulateCorrectData()
        {
            //Arrange
            DURCheckResponse response = new DURCheckResponse();
            List<Rx> currentRxs = new List<Rx>();
            Rx currentRx = new Rx();
            currentRx.MedicationName = "Judytestname";
            currentRx.DosageFormCode = "TAB";
            currentRx.Strength = "5";
            currentRx.StrengthUOM = "MG";
            currentRx.RouteOfAdminCode = "ORAL";
            currentRx.Refills = 1;
            currentRxs.Add(currentRx);


            response.DuplicateTherapy = new DuplicateTherapyResponse();
            response.AlcoholInteractions = new DrugToAlcoholInteractionResponse();
            response.PriorAdverseReactions = new PriorAdverseReactionResonse();
            response.Dosage = new DosageResponse();
            response.DrugInteractions = new DrugToDrugInteractionResponse();

            FoodInteractionResponse foodResponse = new FoodInteractionResponse();
            List<DrugToFoodInteractionLineItem> foodInteractionList = new List<DrugToFoodInteractionLineItem>();
            DrugToFoodInteractionLineItem foodLineItem = new DrugToFoodInteractionLineItem();
            foodLineItem.WarningText = "do not eat grapefruit";
            #region
            foodLineItem.FullWarningText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                "<head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" /><link" +
                " rel=\"stylesheet\" type=\"text/css\" href=\"http://yui.yahooapis.com/2.8.2r1/build/r" +
                "eset/reset-min.css\" /><link href=\"MediSpan.Documents.Monograph.css\" rel=\"stylesh" +
                "eet\" type=\"text/css\" /></head><body><div id=\"title\" class=\"Section\"><table><tr><" +
                "td class=\"SectionTitle Title\">Warfarin Sodium Oral Tablet 2 MG and Food</td></tr" +
                "></table></div><div id=\"codes\" class=\"Section\"><table><tr><td class=\"CodeList\"><" +
                "table><tr><td class=\"CodeTitle\">Management Level</td></tr><tr><td class=\"CodeIte" +
                "m\">Professional Intervention Required</td></tr><tr><td class=\"CodeItem CodeItemS" +
                "elected\">Professional Review Suggested</td></tr><tr><td class=\"CodeItem\">Potenti" +
                "al Interaction Risk</td></tr></table></td><td class=\"CodeList\"><table><tr><td cl" +
                "ass=\"CodeTitle\">Severity Level</td></tr><tr><td class=\"CodeItem CodeItemSelected" +
                "\">Major</td></tr><tr><td class=\"CodeItem\">Moderate</td></tr><tr><td class=\"CodeI" +
                "tem\">Minor</td></tr></table></td><td class=\"CodeList\"><table><tr><td class=\"Code" +
                "Title\">Documentation Level</td></tr><tr><td class=\"CodeItem\">Established</td></t" +
                "r><tr><td class=\"CodeItem CodeItemSelected\">Probable</td></tr><tr><td class=\"Cod" +
                "eItem\">Suspected</td></tr><tr><td class=\"CodeItem\">Possible</td></tr><tr><td cla" +
                "ss=\"CodeItem\">Doubtful/Unknown</td></tr></table></td><td class=\"CodeList\"><table" +
                "><tr><td class=\"CodeTitle\">Labeled Avoidance Level</td></tr><tr><td class=\"CodeI" +
                "tem\">Contraindicated</td></tr><tr><td class=\"CodeItem\">Avoid</td></tr><tr><td cl" +
                "ass=\"CodeItem CodeItemSelected\">Not specified</td></tr></table></td><td class=\"C" +
                "odeList\"><table><tr><td class=\"CodeTitle\">Onset</td></tr><tr><td class=\"CodeItem" +
                " CodeItemSelected\">Delayed</td></tr><tr><td class=\"CodeItem\">Rapid</td></tr></ta" +
                "ble></td><td class=\"CodeList\"><table><tr><td class=\"CodeTitle\">Published Interac" +
                "tion Lists</td></tr><tr><td class=\"CodeItem\">Arizona Center for Education and Re" +
                "search on Therapeutics</td></tr><tr><td class=\"CodeItem\">Beers Criteria</td></tr" +
                "><tr><td class=\"CodeItem\">Office of the National Coordinator</td></tr><tr><td cl" +
                "ass=\"CodeItem\">Pharmacy Quality Alliance</td></tr><tr><td class=\"CodeItem CodeIt" +
                "emSelected\">Not specified</td></tr></table></td></tr></table></div><div id=\"aler" +
                "t\" class=\"Section\"><table><tr><td class=\"CodeTitle\">Alert</td></tr><tr><td><p>Hy" +
                "poprothrombinemic effects of Warfarin Sodium Oral Tablet 2 MG may be decreased b" +
                "y vitamin K-enriched foods or increased by grapefruit or cranberry juice.</p></t" +
                "d></tr></table></div><div id=\"effect\" class=\"Section\"><table><tr><td class=\"Code" +
                "Title\">Effect</td></tr><tr><td><p>Hypoprothrombinemic effects of Warfarin Sodium" +
                " Oral Tablet 2 MG may be decreased by vitamin K-enriched foods or increased by g" +
                "rapefruit or cranberry juice.</p></td></tr></table></div><div id=\"mechanism\" cla" +
                "ss=\"Section\"><table><tr><td class=\"CodeTitle\">Mechanism</td></tr><tr><td><p>Larg" +
                "e quantities of vitamin K from food may competitively inhibit Warfarin Sodium Or" +
                "al Tablet 2 MG binding on end-organ cell receptors. Grapefruit juice, and possib" +
                "ly cranberry juice, may inhibit intestinal CYP3A4 and increase the bioavailabili" +
                "ty of warfarin.</p></td></tr></table></div><div id=\"management\" class=\"Section\">" +
                "<table><tr><td class=\"CodeTitle\">Management</td></tr><tr><td><p>All patients rec" +
                "eiving Warfarin Sodium Oral Tablet 2 MG should be advised to avoid abrupt change" +
                "s in dietary vitamin K content. Large quantities of grapefruit juice or cranberr" +
                "y juice should be avoided. Strict vegetarian diets should be avoided. Monitor in" +
                "ternational normalized ratio and adjust warfarin dosage accordingly.</p></td></t" +
                "r></table></div><div id=\"discussion\" class=\"Section\"><table><tr><td class=\"CodeT" +
                "itle\">Discussion</td></tr><tr><td><p>Acquired warfarin resistance has been linke" +
                "d to high or irregular intake of vitamin K (VK) <span xml:space=\"preserve\" xmlns" +
                "=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#2\">2</a></sup><span xm" +
                "l:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=" +
                "\"#6\">6</a></sup>. Two studies have found high intake of VK or VK-rich foods for " +
                "1, 2, or 7 days interfered with anticoagulation therapy <span xml:space=\"preserv" +
                "e\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#5\">5</a></sup>" +
                "<span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLin" +
                "k\" href=\"#10\">10</a></sup>. Case reports have shown changes in anticoagulant eff" +
                "ect in patients on stable warfarin who began consuming more VK-rich foods <span " +
                "xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" hre" +
                "f=\"#3\">3</a></sup><span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a c" +
                "lass=\"ReferenceLink\" href=\"#4\">4</a></sup><span xml:space=\"preserve\" xmlns=\"\"> <" +
                "/span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#8\">8</a></sup><span xml:spac" +
                "e=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#15\">" +
                "15</a></sup><span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"" +
                "ReferenceLink\" href=\"#18\">18</a></sup><span xml:space=\"preserve\" xmlns=\"\"> </spa" +
                "n><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#19\">19</a></sup>.</p><p>One stud" +
                "y showed that a diet rich in brussels sprouts stimulated warfarin elimination <s" +
                "pan xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\"" +
                " href=\"#7\">7</a></sup>; another showed that food decreased the rate, but not ext" +
                "ent, of warfarin absorption <span xml:space=\"preserve\" xmlns=\"\"> </span><sup xml" +
                "ns=\"\"><a class=\"ReferenceLink\" href=\"#1\">1</a></sup>. In 2 case reports, avocado" +
                ", although low in VK, decreased the effects of warfarin <span xml:space=\"preserv" +
                "e\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#9\">9</a></sup>" +
                ".</p><p>A 44-year-old white male patient with a stable INR had an abrupt decreas" +
                "e of INR from 3.8 to 1.37 <span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns" +
                "=\"\"><a class=\"ReferenceLink\" href=\"#12\">12</a></sup>. He had recently started dr" +
                "inking at least 1/2 gallon of green tea daily. After stopping green tea, INR inc" +
                "reased to 2.6. Green teas may contain large quantities of VK <span xml:space=\"pr" +
                "eserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#13\">13</a" +
                "></sup>.</p><p>INR values in a 70-year-old male patient on stable warfarin for 7" +
                " months decreased from 2.5 to 1.6 after 4 weeks of soy milk and no other dietary" +
                " or drug changes <span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a cl" +
                "ass=\"ReferenceLink\" href=\"#17\">17</a></sup>.</p><p>The effect of frozen grapefru" +
                "it juice (GJ) on PT in 9 patients on stable warfarin doses was studied <span xml" +
                ":space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"" +
                "#11\">11</a></sup>. Patients ingested 240 ml of GJ 3 times/day for 1 week while t" +
                "aking warfarin. There was no significant difference in PT or INR in any patient<" +
                "span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink" +
                "\" href=\"#11\">11</a></sup>. In a separate randomized crossover study of 24 patien" +
                "ts on routine doses of warfarin <span xml:space=\"preserve\" xmlns=\"\"> </span><sup" +
                " xmlns=\"\"><a class=\"ReferenceLink\" href=\"#14\">14</a></sup>, the frequency of dos" +
                "e adjustments in a GJ versus orange juice group were similar. Grapefruit Seed Ex" +
                "tract (GSE) products have been shown to increase INR <span xml:space=\"preserve\" " +
                "xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#29\">29</a></sup>." +
                " Mango was reported to increase INR in 13 patients by an average of 38% <span xm" +
                "l:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=" +
                "\"#16\">16</a></sup>.</p><p>Twenty cases <span xml:space=\"preserve\" xmlns=\"\"> </sp" +
                "an><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#20\">20</a></sup><span xml:space" +
                "=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#22\">2" +
                "2</a></sup><span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"R" +
                "eferenceLink\" href=\"#24\">24</a></sup><span xml:space=\"preserve\" xmlns=\"\"> </span" +
                "><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#25\">25</a></sup><span xml:space=\"" +
                "preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#27\">27<" +
                "/a></sup><span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"Ref" +
                "erenceLink\" href=\"#28\">28</a></sup><span xml:space=\"preserve\" xmlns=\"\"> </span><" +
                "sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#31\">31</a></sup><span xml:space=\"pr" +
                "eserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#39\">39</a" +
                "></sup><span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"Refer" +
                "enceLink\" href=\"#40\">40</a></sup> indicate that increases in INR and death <span" +
                " xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" hr" +
                "ef=\"#31\">31</a></sup> have occurred in patients on warfarin who ingested cranber" +
                "ry juice (CJ) or cranberry sauce <span xml:space=\"preserve\" xmlns=\"\"> </span><su" +
                "p xmlns=\"\"><a class=\"ReferenceLink\" href=\"#30\">30</a></sup><span xml:space=\"pres" +
                "erve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#40\">40</a><" +
                "/sup>. In contrast, 3 well-controlled studies found 250 ml of CJ once to twice d" +
                "aily for 7 to 14 days did not affect the plasma warfarin concentration, PT, and/" +
                "or INR <span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"Refer" +
                "enceLink\" href=\"#26\">26</a></sup><span xml:space=\"preserve\" xmlns=\"\"> </span><su" +
                "p xmlns=\"\"><a class=\"ReferenceLink\" href=\"#32\">32</a></sup><span xml:space=\"pres" +
                "erve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#37\">37</a><" +
                "/sup>. Caution is advised against drinking large quantities of CJ; however, ques" +
                "tions remain regarding the validity of the scientific conclusions being extrapol" +
                "ated to moderate amounts of cranberry juice ingestion <span xml:space=\"preserve\"" +
                " xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#36\">36</a></sup>" +
                ".</p><p>An INR increase has been demonstrated in a patient taking fish oil <span" +
                " xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" hr" +
                "ef=\"#21\">21</a></sup>, pomegranate juice <span xml:space=\"preserve\" xmlns=\"\"> </" +
                "span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#33\">33</a></sup><span xml:spa" +
                "ce=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#34\"" +
                ">34</a></sup>, maitake <span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"" +
                "><a class=\"ReferenceLink\" href=\"#35\">35</a></sup>, Goji juice (L. barbarum) <spa" +
                "n xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink\" h" +
                "ref=\"#41\">41</a></sup>, and black licorice <span xml:space=\"preserve\" xmlns=\"\"> " +
                "</span><sup xmlns=\"\"><a class=\"ReferenceLink\" href=\"#38\">38</a></sup>. A decreas" +
                "ed warfarin effect has been reported with high-protein, low carbohydrate diets <" +
                "span xml:space=\"preserve\" xmlns=\"\"> </span><sup xmlns=\"\"><a class=\"ReferenceLink" +
                "\" href=\"#23\">23</a></sup>.</p></td></tr></table></div><div id=\"references\" class" +
                "=\"Section\"><table><tr><td class=\"CodeTitle\">References</td></tr><tr><td><p><div " +
                "id=\"1\" class=\"Reference\"><span class=\"ReferenceIndex\">1.</span>Musa MN et al.  C" +
                "URR THER RES. 1976; Vol. 20:630.</div><div id=\"2\" class=\"Reference\"><span class=" +
                "\"ReferenceIndex\">2.</span>Kelly JG et al.  Clin Pharmacokinet. 1979; Vol. 4:1.36" +
                "9763</div><div id=\"3\" class=\"Reference\"><span class=\"ReferenceIndex\">3.</span>Qu" +
                "reshi GD et al.  Arch Intern Med. 1981; Vol. 141:507.7212893</div><div id=\"4\" cl" +
                "ass=\"Reference\"><span class=\"ReferenceIndex\">4.</span>Walker FB.  Arch Intern Me" +
                "d. 1984; Vol. 144:2089.6486994</div><div id=\"5\" class=\"Reference\"><span class=\"R" +
                "eferenceIndex\">5.</span>Karlson B et al.  Acta Med Scand. 1986; Vol. 220:347.354" +
                "1503</div><div id=\"6\" class=\"Reference\"><span class=\"ReferenceIndex\">6.</span>Ke" +
                "arns PJ et al.  JPEN. 1986; Vol. 10:100.3945042</div><div id=\"7\" class=\"Referenc" +
                "e\"><span class=\"ReferenceIndex\">7.</span>Ovesen L et al.  Eur J Clin Pharmacol. " +
                "1988; Vol. 33:521.3203715</div><div id=\"8\" class=\"Reference\"><span class=\"Refere" +
                "nceIndex\">8.</span>Chow WH et al.  Postgrad Med J. 1990; Vol. 66:855.2099431</di" +
                "v><div id=\"9\" class=\"Reference\"><span class=\"ReferenceIndex\">9.</span>Blickstein" +
                " D et al.  [letter]. Lancet. 1991; Vol. 337:914.</div><div id=\"10\" class=\"Refere" +
                "nce\"><span class=\"ReferenceIndex\">10.</span>Pedersen FM et al.  J Intern Med. 19" +
                "91; Vol. 229:517.2045759</div><div id=\"11\" class=\"Reference\"><span class=\"Refere" +
                "nceIndex\">11.</span>Sullivan DM et al.  Am J Health-Syst Pharm. 1998; Vol. 55:15" +
                "81.9706183</div><div id=\"12\" class=\"Reference\"><span class=\"ReferenceIndex\">12.<" +
                "/span>Taylor JR et al.  Ann Pharmacother. 1999; Vol. 33:426.10332534</div><div i" +
                "d=\"13\" class=\"Reference\"><span class=\"ReferenceIndex\">13.</span>Booth SL et al. " +
                " J Agric Food Chem. 1995; Vol. 43:1574.</div><div id=\"14\" class=\"Reference\"><spa" +
                "n class=\"ReferenceIndex\">14.</span>Dresser GK et al.  [abstract]. Clin Pharmacol" +
                " Ther. 1999; Vol. 65:193.</div><div id=\"15\" class=\"Reference\"><span class=\"Refer" +
                "enceIndex\">15.</span>Bartle WR et al.  [letter]. Am J Health-Syst Pharm. 2001; V" +
                "ol. 58:2300.</div><div id=\"16\" class=\"Reference\"><span class=\"ReferenceIndex\">16" +
                ".</span>Monterrey-Rodriguez J et al.  [letter]. Ann Pharmacother. 2002; Vol. 36:" +
                "940.</div><div id=\"17\" class=\"Reference\"><span class=\"ReferenceIndex\">17.</span>" +
                "Cambria-Kiely JA.  Ann Pharmacother. 2002; Vol. 36:1893.12452752</div><div id=\"1" +
                "8\" class=\"Reference\"><span class=\"ReferenceIndex\">18.</span>Kudo T.  Artery. 199" +
                "0; Vol. 17:189.2360879</div><div id=\"19\" class=\"Reference\"><span class=\"Referenc" +
                "eIndex\">19.</span>Ohkawa S et al.  Rinsho Shinkeigaku. 1995; Vol. 35:806.8777808" +
                "</div><div id=\"20\" class=\"Reference\"><span class=\"ReferenceIndex\">20.</span>Suva" +
                "rna R et al.  Br Med J. 2003; Vol. 327:1454.14684645</div><div id=\"21\" class=\"Re" +
                "ference\"><span class=\"ReferenceIndex\">21.</span>Buckley MS et al.  Ann Pharmacot" +
                "her. 2004; Vol. 38:50.14742793</div><div id=\"22\" class=\"Reference\"><span class=\"" +
                "ReferenceIndex\">22.</span>Grant P.  J Heart Valve Dis. 2004; Vol. 13:25.14765835" +
                "</div><div id=\"23\" class=\"Reference\"><span class=\"ReferenceIndex\">23.</span>Beat" +
                "ty SJ et al.  Ann Pharmacother. 2005; Vol. 29:744.15755790</div><div id=\"24\" cla" +
                "ss=\"Reference\"><span class=\"ReferenceIndex\">24.</span>Sylvan L et al.  [letter]." +
                " Am Fam Physician. 2005; Vol. 72:1000.</div><div id=\"25\" class=\"Reference\"><span" +
                " class=\"ReferenceIndex\">25.</span>Rindone JP et al.  Am J Ther. 2005; Vol. 13:28" +
                "3.</div><div id=\"26\" class=\"Reference\"><span class=\"ReferenceIndex\">26.</span>Zh" +
                "aoping Li et al.  J Am Diet Assoc. 2006; Vol. 106:12.17126638</div><div id=\"27\" " +
                "class=\"Reference\"><span class=\"ReferenceIndex\">27.</span>Welch JM et al.  J Phar" +
                "m Technol. 2007; Vol. 23:104.</div><div id=\"28\" class=\"Reference\"><span class=\"R" +
                "eferenceIndex\">28.</span>Paeng CH et al.  Clin Ther. 2007; Vol. 29:1730.17919554" +
                "</div><div id=\"29\" class=\"Reference\"><span class=\"ReferenceIndex\">29.</span>Bran" +
                "din H et al.  Eur J Clin Pharmacol. 2007; Vol. 63:565.17468864</div><div id=\"30\"" +
                " class=\"Reference\"><span class=\"ReferenceIndex\">30.</span>Mergenhagen KA et al. " +
                " Am J Health Syst Pharm. 2008; Vol. 65:2113.18997138</div><div id=\"31\" class=\"Re" +
                "ference\"><span class=\"ReferenceIndex\">31.</span>Griffiths AP et al.  J R Soc Hea" +
                "lth. 2008; Vol. 128:324.19058474</div><div id=\"32\" class=\"Reference\"><span class" +
                "=\"ReferenceIndex\">32.</span>Ansell J et al.  J Clin Pharmacol. 2009; Vol. 49:824" +
                ".6712412</div><div id=\"33\" class=\"Reference\"><span class=\"ReferenceIndex\">33.</s" +
                "pan>Komperda KE.  Pharmacotherapy. 2009; Vol. 29:1002.19637955</div><div id=\"34\"" +
                " class=\"Reference\"><span class=\"ReferenceIndex\">34.</span>Jarvis S et al.  Emerg" +
                " Med J. 2010; Vol. 27:74.20029019</div><div id=\"35\" class=\"Reference\"><span clas" +
                "s=\"ReferenceIndex\">35.</span>Hanselin MR.  Ann Pharmacother. 2010; Vol. 44:223.2" +
                "0040699</div><div id=\"36\" class=\"Reference\"><span class=\"ReferenceIndex\">36.</sp" +
                "an>Zikria J et al.  Am J Med. 2010; Vol. 123:384.20399311</div><div id=\"37\" clas" +
                "s=\"Reference\"><span class=\"ReferenceIndex\">37.</span>Mellen CK et al.  Br J Clin" +
                " Pharmacol. 2010; Vol. 70:139.20642557</div><div id=\"38\" class=\"Reference\"><span" +
                " class=\"ReferenceIndex\">38.</span>Liu JF et al.  World J Gastrointest Surg. 2010" +
                "; Vol. 2:30.21160832PMC2999197</div><div id=\"39\" class=\"Reference\"><span class=\"" +
                "ReferenceIndex\">39.</span>Hamann GL et al.  Ann Pharmacother. 2011; Vol. 45:e17." +
                "21364039</div><div id=\"40\" class=\"Reference\"><span class=\"ReferenceIndex\">40.</s" +
                "pan>Haber SL et al.  Consult Pharm. 2012; Vol. 27:58.22231999</div><div id=\"41\" " +
                "class=\"Reference\"><span class=\"ReferenceIndex\">41.</span>Rivera CA et al.  Pharm" +
                "acotherapy. 2012; Vol. 32:e50.22392461</div></p></td></tr></table></div><div id=" +
                "\"footer\" class=\"Section Footer\"><div id=\"disclaimer\" class=\"SectionContent Discl" +
                "aimer\"><p>The information contained in the Clinical Drug Information, LLC databa" +
                "ses is intended to supplement the knowledge of physicians, pharmacists, and othe" +
                "r healthcare professionals regarding drug therapy problems and patient counselin" +
                "g information. This information is advisory only and is not intended to replace " +
                "sound clinical judgment in the delivery of healthcare services.</p><p>Clinical D" +
                "rug Information, LLC disclaims all warranties, whether expressed or implied, inc" +
                "luding any warranty as to the quality, accuracy, and suitability of this informa" +
                "tion for any purpose.</p></div><div id=\"copyright\" class=\"SectionContent Copyrig" +
                "ht\"><p>Copyright 2017 Clinical Drug Information, LLC and its affiliates and/or l" +
                "icensors.  All rights reserved.</p></div></div></body></html>";
            #endregion

            foodInteractionList.Add(foodLineItem);
            foodResponse.Interactions = foodInteractionList;
            response.FoodInteractions = foodResponse;

            //Act

            DataSet dset = DURMedispanWarningAdapter.PopulateResponseData(currentRxs, response);
          
            string foodInteractionReport = "FOOD_INTERACTION Report:\r\n-----------------------------\r\n\r\nName: Judytestname\r\nHypoprothrombinemic effects of Warfarin Sodium Oral Tablet 2 MG may be decreased by vitamin K-enriched foods or increased by grapefruit or cranberry juice.\r\n\r\nEffect: Hypoprothrombinemic effects of Warfarin Sodium Oral Tablet 2 MG may be decreased by vitamin K-enriched foods or increased by grapefruit or cranberry juice.\r\n\r\nMechanism: Large quantities of vitamin K from food may competitively inhibit Warfarin Sodium Oral Tablet 2 MG binding on end-organ cell receptors. Grapefruit juice, and possibly cranberry juice, may inhibit intestinal CYP3A4 and increase the bioavailability of warfarin.\r\n\r\nManagement: All patients receiving Warfarin Sodium Oral Tablet 2 MG should be advised to avoid abrupt changes in dietary vitamin K content. Large quantities of grapefruit juice or cranberry juice should be avoided. Strict vegetarian diets should be avoided. Monitor international normalized ratio and adjust warfarin dosage accordingly.\r\n\r\nDiscussion: Acquired warfarin resistance has been linked to high or irregular intake of vitamin K (VK)  2 6. Two studies have found high intake of VK or VK-rich foods for 1, 2, or 7 days interfered with anticoagulation therapy  5 10. Case reports have shown changes in anticoagulant effect in patients on stable warfarin who began consuming more VK-rich foods  3 4 8 15 18 19.One study showed that a diet rich in brussels sprouts stimulated warfarin elimination  7; another showed that food decreased the rate, but not extent, of warfarin absorption  1. In 2 case reports, avocado, although low in VK, decreased the effects of warfarin  9.A 44-year-old white male patient with a stable INR had an abrupt decrease of INR from 3.8 to 1.37  12. He had recently started drinking at least 1/2 gallon of green tea daily. After stopping green tea, INR increased to 2.6. Green teas may contain large quantities of VK  13.INR values in a 70-year-old male patient on stable warfarin for 7 months decreased from 2.5 to 1.6 after 4 weeks of soy milk and no other dietary or drug changes  17.The effect of frozen grapefruit juice (GJ) on PT in 9 patients on stable warfarin doses was studied  11. Patients ingested 240 ml of GJ 3 times/day for 1 week while taking warfarin. There was no significant difference in PT or INR in any patient 11. In a separate randomized crossover study of 24 patients on routine doses of warfarin  14, the frequency of dose adjustments in a GJ versus orange juice group were similar. Grapefruit Seed Extract (GSE) products have been shown to increase INR  29. Mango was reported to increase INR in 13 patients by an average of 38%  16.Twenty cases  20 22 24 25 27 28 31 39 40 indicate that increases in INR and death  31 have occurred in patients on warfarin who ingested cranberry juice (CJ) or cranberry sauce  30 40. In contrast, 3 well-controlled studies found 250 ml of CJ once to twice daily for 7 to 14 days did not affect the plasma warfarin concentration, PT, and/or INR  26 32 37. Caution is advised against drinking large quantities of CJ; however, questions remain regarding the validity of the scientific conclusions being extrapolated to moderate amounts of cranberry juice ingestion  36.An INR increase has been demonstrated in a patient taking fish oil  21, pomegranate juice  33 34, maitake  35, Goji juice (L. barbarum)  41, and black licorice  38. A decreased warfarin effect has been reported with high-protein, low carbohydrate diets  23.\r\n\r\nReferences: 1.Musa MN et al.  CURR THER RES. 1976; Vol. 20:630.2.Kelly JG et al.  Clin Pharmacokinet. 1979; Vol. 4:1.3697633.Qureshi GD et al.  Arch Intern Med. 1981; Vol. 141:507.72128934.Walker FB.  Arch Intern Med. 1984; Vol. 144:2089.64869945.Karlson B et al.  Acta Med Scand. 1986; Vol. 220:347.35415036.Kearns PJ et al.  JPEN. 1986; Vol. 10:100.39450427.Ovesen L et al.  Eur J Clin Pharmacol. 1988; Vol. 33:521.32037158.Chow WH et al.  Postgrad Med J. 1990; Vol. 66:855.20994319.Blickstein D et al.  [letter]. Lancet. 1991; Vol. 337:914.10.Pedersen FM et al.  J Intern Med. 1991; Vol. 229:517.204575911.Sullivan DM et al.  Am J Health-Syst Pharm. 1998; Vol. 55:1581.970618312.Taylor JR et al.  Ann Pharmacother. 1999; Vol. 33:426.1033253413.Booth SL et al.  J Agric Food Chem. 1995; Vol. 43:1574.14.Dresser GK et al.  [abstract]. Clin Pharmacol Ther. 1999; Vol. 65:193.15.Bartle WR et al.  [letter]. Am J Health-Syst Pharm. 2001; Vol. 58:2300.16.Monterrey-Rodriguez J et al.  [letter]. Ann Pharmacother. 2002; Vol. 36:940.17.Cambria-Kiely JA.  Ann Pharmacother. 2002; Vol. 36:1893.1245275218.Kudo T.  Artery. 1990; Vol. 17:189.236087919.Ohkawa S et al.  Rinsho Shinkeigaku. 1995; Vol. 35:806.877780820.Suvarna R et al.  Br Med J. 2003; Vol. 327:1454.1468464521.Buckley MS et al.  Ann Pharmacother. 2004; Vol. 38:50.1474279322.Grant P.  J Heart Valve Dis. 2004; Vol. 13:25.1476583523.Beatty SJ et al.  Ann Pharmacother. 2005; Vol. 29:744.1575579024.Sylvan L et al.  [letter]. Am Fam Physician. 2005; Vol. 72:1000.25.Rindone JP et al.  Am J Ther. 2005; Vol. 13:283.26.Zhaoping Li et al.  J Am Diet Assoc. 2006; Vol. 106:12.1712663827.Welch JM et al.  J Pharm Technol. 2007; Vol. 23:104.28.Paeng CH et al.  Clin Ther. 2007; Vol. 29:1730.1791955429.Brandin H et al.  Eur J Clin Pharmacol. 2007; Vol. 63:565.1746886430.Mergenhagen KA et al.  Am J Health Syst Pharm. 2008; Vol. 65:2113.1899713831.Griffiths AP et al.  J R Soc Health. 2008; Vol. 128:324.1905847432.Ansell J et al.  J Clin Pharmacol. 2009; Vol. 49:824.671241233.Komperda KE.  Pharmacotherapy. 2009; Vol. 29:1002.1963795534.Jarvis S et al.  Emerg Med J. 2010; Vol. 27:74.2002901935.Hanselin MR.  Ann Pharmacother. 2010; Vol. 44:223.2004069936.Zikria J et al.  Am J Med. 2010; Vol. 123:384.2039931137.Mellen CK et al.  Br J Clin Pharmacol. 2010; Vol. 70:139.2064255738.Liu JF et al.  World J Gastrointest Surg. 2010; Vol. 2:30.21160832PMC299919739.Hamann GL et al.  Ann Pharmacother. 2011; Vol. 45:e17.2136403940.Haber SL et al.  Consult Pharm. 2012; Vol. 27:58.2223199941.Rivera CA et al.  Pharmacotherapy. 2012; Vol. 32:e50.22392461The information contained in the Clinical Drug Information, LLC databases is intended to supplement the knowledge of physicians, pharmacists, and other healthcare professionals regarding drug therapy problems and patient counseling information. This information is advisory only and is not intended to replace sound clinical judgment in the delivery of healthcare services.Clinical Drug Information, LLC disclaims all warranties, whether expressed or implied, including any warranty as to the quality, accuracy, and suitability of this information for any purpose.\r\n\r\nCopyright  2017 Clinical Drug Information, LLC and its affiliates and/or licensors.  All rights reserved.";

            //Assert


            Assert.AreEqual(foodInteractionReport, dset.Tables[1].Rows[0]["WarningText"].ToString());  
            Assert.AreEqual(DURWarningType.FOOD_INTERACTION, (DURWarningType)dset.Tables[1].Rows[0]["WarningType"]);
        }

        [TestMethod]
        public void FilterResponseShouldFilterOutDelayedFoodInteractionResponseWhenUserSettingsRapid ()
        {
            //Arrange
            DURCheckResponse response = new DURCheckResponse();
            FoodInteractionResponse foodResponse = new FoodInteractionResponse();
            List<DrugToFoodInteractionLineItem> foodInteractionList = new List<DrugToFoodInteractionLineItem>();
            DrugToFoodInteractionLineItem foodLineItem = new DrugToFoodInteractionLineItem();
            foodLineItem.WarningText = "do not eat grapefruit";
            foodLineItem.Onset = "Delayed";

            foodInteractionList.Add(foodLineItem);
            foodResponse.Interactions = foodInteractionList;
            response.FoodInteractions = foodResponse;
            DURSettings settings = new DURSettings();
            settings.InteractionOnsetCheckType = InteractionOnset.Rapid;

            //Act
            MedispanServiceBroker.FilterFoodResponse( response,  settings);

            //Assert
            Assert.AreEqual(response.FoodInteractions.Interactions.Count, 0);
        }

        [TestMethod]
        public void FilterResponseShouldNotFilterOutDelayedFoodInteractionResponseWhenUserSettingsDelayed()
        {
            //Arrange
            DURCheckResponse response = new DURCheckResponse();
            FoodInteractionResponse foodResponse = new FoodInteractionResponse();
            List<DrugToFoodInteractionLineItem> foodInteractionList = new List<DrugToFoodInteractionLineItem>();
            DrugToFoodInteractionLineItem foodLineItem = new DrugToFoodInteractionLineItem();
            foodLineItem.WarningText = "do not eat grapefruit";
            foodLineItem.Onset = "Delayed";

            foodInteractionList.Add(foodLineItem);
            foodResponse.Interactions = foodInteractionList;
            response.FoodInteractions = foodResponse;
            DURSettings settings = new DURSettings();
            settings.InteractionOnsetCheckType = InteractionOnset.Delayed;

            //Act
            MedispanServiceBroker.FilterFoodResponse(response, settings);

            //Assert
            Assert.AreEqual(response.FoodInteractions.Interactions.Count, 1);
        }

        [TestMethod]
        public void FilterResponseShouldFilterOutDelayedAlcoholInteractionResponseWhenUserSettingsRapid()
        {
            //Arrange
            DURCheckResponse response = new DURCheckResponse();
            DrugToAlcoholInteractionResponse alcResponse = new DrugToAlcoholInteractionResponse();
            List<DrugToAlcoholInteractionLineItem> alcInteractionList = new List<DrugToAlcoholInteractionLineItem>();
            DrugToAlcoholInteractionLineItem alcLineItem = new DrugToAlcoholInteractionLineItem();
            alcLineItem.WarningText = "do not drink flat wine";
            alcLineItem.Onset = Constants.DURInteractionOnsetType.Delayed;

            DrugToAlcoholInteractionLineItem alcLineItem2 = new DrugToAlcoholInteractionLineItem();
            alcLineItem2.WarningText = "do not drink cheap wine";
            alcLineItem2.Onset = Constants.DURInteractionOnsetType.Rapid;


            alcInteractionList.Add(alcLineItem);
            alcInteractionList.Add(alcLineItem2);
            alcResponse.Interactions = alcInteractionList;
            response.AlcoholInteractions = alcResponse;
            DURSettings settings = new DURSettings();
            settings.InteractionOnsetCheckType = InteractionOnset.Rapid;

            //Act
            MedispanServiceBroker.FilterAlcoholResponse(response, settings);

            //Assert
            Assert.AreEqual(response.AlcoholInteractions.Interactions[0].Onset, Constants.DURInteractionOnsetType.Rapid);
        }

        [TestMethod]
        public void FilterResponseShouldFilterOutDelayedDrugInteractionResponseWhenUserSettingsRapid()
        {
            //Arrange
            DURCheckResponse response = new DURCheckResponse();
            DrugToDrugInteractionResponse drugResponse = new DrugToDrugInteractionResponse();
            List<DrugToDrugInteractionLineItem> drugInteractionList = new List<DrugToDrugInteractionLineItem>();
            DrugToDrugInteractionLineItem drugLineItem = new DrugToDrugInteractionLineItem();
            drugLineItem.WarningText = "do not take this while you are taking that";
            drugLineItem.Onset = "Delayed";

            DrugToDrugInteractionLineItem drugLineItem2 = new DrugToDrugInteractionLineItem();
            drugLineItem2.WarningText = "do not take this while you are taking that unless you want to";
            drugLineItem2.Onset = "Rapid";



            drugInteractionList.Add(drugLineItem);
            drugInteractionList.Add(drugLineItem2);
            drugResponse.Interactions = drugInteractionList;
            response.DrugInteractions = drugResponse;
            DURSettings settings = new DURSettings();
            settings.InteractionOnsetCheckType = InteractionOnset.Rapid;

            //Act
            MedispanServiceBroker.FilterDrugDrugResponse(response, settings);

            //Assert
            Assert.AreEqual(response.DrugInteractions.Interactions[0].Onset, "Rapid");
        }

    }
}
