using log4net.Layout;
using System.IO;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Collections;

namespace iMFAS.Services.Logger
{
    /// <summary>
    /// This used for log as Json
    /// </summary>
    public class JSONLayout : LayoutSkeleton
    {
        public JSONLayout()
        {
            this.IgnoresException = false;
        }

        override public void ActivateOptions()
        {
            // nothing to do
        }

        override public void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            // Construct the map based on the string formed by the MessageObject...
            IDictionary<string, string> nameValueMap = extractNameValuePairs(loggingEvent.MessageObject.ToString(), ", ", " = ");
            
            // Add Level and Timestamp...
            nameValueMap.Add("level", loggingEvent.Level.DisplayName);
            nameValueMap.Add("timestamp", loggingEvent.TimeStamp.ToString());

            // Serialize the map into a String...
            string json = serializeMap(nameValueMap);

            // Write out the length of the string...
            writer.Write(json.Length);
            writer.WriteLine();

            // Write out the actual JSON...
            writer.Write(json);
            writer.WriteLine();
        }

        private IDictionary<string, string> extractNameValuePairs(string combinedString, string delimiter1, string delimiter2) {
            // Create a Dictionary to hold our name, value pairs...
            IDictionary<string, string> map = new Dictionary<string, string>();

            // Start by splitting the overall string by the first delimiter...
            string[] parts = combinedString.Split(new string[] { delimiter1 }, StringSplitOptions.None);

            // Loop through the parts...
            string lastName = null;
            for (int i = 0; i < parts.Length; i++)
            {
                // Try to split the "part" into name and value pairs...
                string[] nameValue = parts[i].Split(new string[] { delimiter2 }, StringSplitOptions.None);

                // Do we have exactly 2 parts?
                if (nameValue.Length == 2)
                {
                    // Add the name/value pair to our map...
                    map.Add(nameValue[0], nameValue[1]);

                    // Remember this name as being the last one we encountered...
                    lastName = nameValue[0];
                }
                // Do we have only 1 part?
                else if (nameValue.Length == 1)
                {
                    string newValue;

                    // Do we have a previously held name?
                    if (lastName != null)
                    {
                        // What we have is a continuation of the previous named value
                        newValue = map[lastName] + delimiter1 + nameValue[0];
                    }
                    else
                    {
                        // We have the case where our first part has no name
                        lastName = "UNNAMED";

                        // Our new value is the full string...
                        newValue = nameValue[0];
                    }

                    // Assign it back to our map...
                    map[lastName] = newValue;
                }
                // We have a value with has a least one second level delimiter in the value...
                else
                {
                    // The first element is the name...
                    string name = nameValue[0];

                    // The rest need to be reassembled into a single value...
                    string value = String.Join(delimiter2, new ArraySegment<string>(nameValue, 1, nameValue.Length - 1).Array);

                    // Add the name/value pair to our map...
                    map.Add(name, value);
                }
            }

            return map;
        }

        private string serializeMap(IDictionary<string, string> nameValueMap)
        {
            // Populate our array of name/value pairs...
            ArrayList nameValuePairs = new ArrayList();

            // ...by traversing the map...
            foreach (string name in nameValueMap.Keys)
            {
                // Get the value...
                string value = nameValueMap[name];

                // Add to our array...
                nameValuePairs.Add("\"" + name + "\"" + ": " + "\"" + value + "\"");
            }

            // Return the name value pairs joined by commas and bracketed by braces...
            return "{" + String.Join(", ", nameValuePairs.ToArray()) + "}";
        }
    }
}