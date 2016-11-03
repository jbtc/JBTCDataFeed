using OPCSystemsDataConnector;
using OPCControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace jbt_opc
{
    internal class ClassTagValues
    {
        internal string[] TagNames;
        internal object[] Values;
        internal bool[] Qualities;
        internal DateTime[] TimeStamps;
        

        public ClassTagValues(string[] NewTagNames, object[] NewValues, bool[] NewQualities, DateTime[] NewTimeStamps)
        {
            TagNames = NewTagNames;
            Values = NewValues;
            Qualities = NewQualities;
            TimeStamps = NewTimeStamps;
        }
    }

    public class JBTOPC
    {
        //internal OPCSystemsDataConnector.OPCSystemsData osd ;
        public volatile OPCControlsData OPCDataComponent1;
        private volatile string[] tags;
        public volatile string OASServer = @"\\10.204.152.11\";
        public List<string> TagUpdates = new List<string>();
        private List<string> _tagUpdatesbuffer = new List<string>();
        public bool canAddTagstoReadQueue = true;

        /// <summary>
        /// prepare all tags and add to subscription
        /// </summary>
        public JBTOPC( List<string> tagNames, string oasserver)
        {
            OASServer = oasserver;
            if (tagNames.Count > 0)
            {
                tags = new string[tagNames.Count];
                int tindex = 0;
                foreach (string set in tagNames)
                {
                    tags[tindex] = set;
                    tindex++;
                }

                OPCDataComponent1 = new OPCControlsData();

                OPCDataComponent1.AddTags(tags);
            }
            else
            {
                // no tag list configured
                // get tag list from opc server
                
                string errorString = "NONE";
                string[] CSVStrings = getTagsFromOPCServer(out errorString);
                if (errorString == "Success")
                {
                    OPC_TAG_Collection tset = new OPC_TAG_Collection(CSVStrings);
                    #region test
                    //foreach (string set in CSVStrings)
                    //{
                    //    //log for now
                    //    File.WriteAllText(@".\out.txt", set);
                    //}
                    #endregion

                    tags = new string[tset.ValueCollection.Count];
                    int tindex = 0;
                    foreach (OPC_Tag set in tset.ValueCollection)
                    {
                        //log for now
                        string res = "Tag: "+set.Tag +"### dataType: "+set.Value_Data_Type +"### value: "+set.Value_Value +"### descr: "+set.Value_Desc + Environment.NewLine;
                        string parentfolder = @"C:\";//Environment.GetFolderPath(SpecialFolder.ApplicationData);
                        string filePath = parentfolder + @"\JBTAerotech\" + ConfigurationManager.AppSettings["AppName"] + @"\" + DateTime.Now.ToString("MMddyyyy") + @"\TagsLog.txt";

                        File.AppendAllText(filePath, res);
                        tags[tindex] = set.Tag.Replace("\"","")+".Value";
                        tindex++;
                    }

                    OPCDataComponent1 = new OPCControlsData();

                    OPCDataComponent1.AddTags(tags);

                    OPCDataComponent1.ValuesChangedAll  += new OPCControlsData.ValuesChangedAllEventHandler(OPCDataComponent1_ValuesChangedAll);
                }
                else
                {
                    // not much we can do here 
                    // we have to check the taglist count outside
                    // to verify if we were able to get something from the oas server
                    
                }
                // we do not want to pull the tag list every time we start
                // we need to verify the version of the latest tag list that is stored
                // we need to be able to store tag-list versions

                

            }



            
        }

        
         



        /// <summary>
        /// remove all tags from subscription
        /// </summary>
        ~JBTOPC()
        {
            OPCDataComponent1.RemoveTags(tags);
            tags = null;
        }

        /// <summary>
        /// Gets the tags from opc server.
        /// </summary>
        /// <param name="errorString">The error string.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private string[] getTagsFromOPCServer(out string errorString)
        {
            string ErrorString = "";
            string ip = OASServer.Replace(@"\", "");

            // did not work with given tag description from opc
            //string[] CSVStrings = ModuleNetworkNode.OPCSystemsComponent1.TagCSVExport(ip, ref ErrorString);
            string[] DesiredTags = new string[4];
            DesiredTags[0] = "Tag";
            DesiredTags[1] = "Value - Data Type";
            DesiredTags[2] = "Value - Value";
            DesiredTags[3] = "Value - Desc";
                   


            string[] CSVStrings = ModuleNetworkNode.OPCSystemsComponent1.TagCSVExportWithDesiredColumns(DesiredTags, ip, ref ErrorString);
            errorString = ErrorString;
            return CSVStrings;
        }

        /// <summary>
        /// write tag with result - float
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="tagvalue"></param>
        /// <returns></returns>
        public string WriteTag(string tagname, double tagvalue)
        {
            return WriteTags(tagname,tagvalue);
        }

        /// <summary>
        /// write tag with result - int
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="tagvalue"></param>
        /// <returns></returns>
        public string WriteTag(string tagname, int tagvalue)
        {
            return WriteTags(tagname, tagvalue);
        }

        /// <summary>
        /// write tag with result - string
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="tagvalue"></param>
        /// <returns></returns>
        public string WriteTag(string tagname, string tagvalue)
        {
            return WriteTags(tagname, tagvalue);
        }

        /// <summary>
        /// universal write tag
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="tagvalue"></param>
        /// <returns></returns>
        public string WriteTags(string tagname, object tagvalue)
        {
            string writeResult = "NONE";
            /*
             * excerpt from opc help .. 
             * The Errors array will be sized to the same length as your Tags array.
             * The following values will be returned to you in the Int32 array you pass.
             * 0 = Good Quality
             * 1 = Service was not reachable for writing
             * 2 = Timeout occurred on value, the value returned was never equal to the desired value or within the deadband for Double and Single values within the timeout period 
             * 3 = Tag array did not match the size of the Values array
             * The Timeout value is specified in milliseconds
             * The FloatDeadband is for comparing only Double and Single values.
             */

            string[] tags = new string[1];
            tags[0] = OASServer + tagname + ".Value";
            object[] values = new object[1];
            values[0] = tagvalue;

            // int array 
            var Errors = OPCDataComponent1.SyncWriteTagsWithConfirmation(tags, values, 10000, 0.0001);
            writeResult = "";
            foreach (int res in Errors)
            {
                
                switch (res)
                {
                    case 0:
                        writeResult += "Good Quality";
                        break;
                    case 1:
                        writeResult += "Service was not reachable for writing";
                        break;
                    case 2:
                        writeResult += "Timeout occurred on value, the value returned was never equal to the desired value or within the deadband for Double and Single values within the timeout period ";
                        break;
                    case 3:
                        writeResult += "Tag array did not match the size of the Values array";
                        break;

                    default:
                        writeResult += "not registered status: "+res.ToString();
                        break;
                }
                writeResult += "|";
            }
            return writeResult;
        }


        /// <summary>
        /// This event fires when the values of the Tags change.
        /// </summary>
        /// <param name="Tags"></param>
        /// <param name="Values"></param>
        /// <param name="Qualities"></param>
        /// <param name="TimeStamps"></param>
        private void OPCDataComponent1_ValuesChangedAll(string[] Tags, object[] Values, bool[] Qualities, DateTime[] TimeStamps)
        {
            if ((canAddTagstoReadQueue == true) && 
                (_tagUpdatesbuffer.Count >  0 )
                )
            {
                TagUpdates.AddRange(_tagUpdatesbuffer.ToArray());
                _tagUpdatesbuffer.Clear();
            }
                int g = 0;
            foreach (string t in Tags)
            {
                
                //log for now                
                string res = "Tag:" + t + 
                    "#Value:" + (Values[g] == null ? "null" : Values[g].ToString()) + 
                    "#Quality:" + (Qualities[g] == true ? "True" : "False") + 
                    "#Time Stamp:" + (TimeStamps[g] == null ? "NONE" : TimeStamps[g].ToString()) + Environment.NewLine;
                //string parentfolder = @"C:\";
                //string filePath = parentfolder + @"\JBTAerotech\" + ConfigurationManager.AppSettings["AppName"] + @"\" + DateTime.Now.ToString("MMddyyyy") + @"\outChangesLog.txt";
                //File.AppendAllText(filePath, res);
                if (canAddTagstoReadQueue == true)
                {
                    TagUpdates.Add(res);
                }
                else
                {
                    _tagUpdatesbuffer.Add(res);
                }
                g++;
            }
            // High speed version with a lot of data values changing
            //lock (m_DataValuesQueue.SyncRoot)
            // {
            //     m_DataValuesQueue.Enqueue(new ClassTagValues(Tags, Values, Qualities, TimeStamps));
            // }

            // You can use the values directly here within the data event, but the example shown above with a Queue will work best with thousands of tags updating evey second.
            //' Simple version of just obtaining the tag value you are interested in.
            //Dim TagIndex As Int32
            //Dim NumberOfTagValues As Int32 = Tags.GetLength(0)
            //For TagIndex = 0 To NumberOfTagValues - 1
            //    Select Case Tags(TagIndex)
            //        Case "Ramp.Value"
            //            If Qualities(TagIndex) Then
            //                ' The value of Ramp.Value is contained in Values(TagIndex)
            //            Else
            //                ' The value of Ramp.Value is bad
            //            End If
            //    End Select
            //Next
        }


    }
}
