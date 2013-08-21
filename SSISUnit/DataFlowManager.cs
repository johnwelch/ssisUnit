using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

namespace SsisUnit
{
    public class DataFlowManager
    {
        private const string ManagedComponentId = "{874F7595-FB5F-40FF-96AF-FBFF8250E3EF}";

        private const string DummySourceTypeName = "PW.TaskFactory.DummySource.DummySource";

        private const string DummyTargetName = "PW.TaskFactory.TerminatorDestination.TerminatorDestination";

        private readonly Dictionary<int, int> _flowletIdMapping = new Dictionary<int, int> { { 0, 0 } };
        private readonly Dictionary<int, int> _sourceColumnMappings = new Dictionary<int, int>();

        // TODO: Add overloads that take a path object (can replace that path with new components - path gives us source and sink)
        public Package InsertFlowlet(Package package, MainPipe pipeline, MainPipe flowlet, IDTSOutput100 source, IDTSInput100 sink, Dictionary<string, string> sourceMapping, Dictionary<string, string> sinkMapping)
        {
            var events = new ComponentEventHandler();
            //pipeline.Events = DtsConvert.GetExtendedInterface(events);

            IDTSInput100 flowletStart = FindStart(flowlet, source, sourceMapping);
            IDTSOutput100 flowletEnd = FindEnd(flowlet, sink, sinkMapping);

            foreach (IDTSOutputColumn100 outputColumn in source.OutputColumnCollection)
            {
                string targetMapping;
                if (sourceMapping.TryGetValue(outputColumn.IdentificationString, out targetMapping))
                {
                    _sourceColumnMappings.Add(GetIdForRefId(flowlet, targetMapping), outputColumn.ID);
                }
            }

            foreach (IDTSComponentMetaData100 component in flowlet.ComponentMetaDataCollection)
            {
                // Exclude Source and Target components, but capture them for future
                // pipeline.AutoGenerateIDForNewObjects - interesting possibilities
                if (!(component.ComponentClassID == ManagedComponentId &&
                    component.CustomPropertyCollection["UserComponentTypeName"].Value == DummySourceTypeName))
                {
                    CloneComponent(pipeline, component);
                }
            }

            // Remove original paths
            if (source.IsAttached)
            {
                var path = pipeline.PathCollection.Cast<IDTSPath100>().First(item => item.StartPoint.ID == source.ID);
                pipeline.PathCollection.RemoveObjectByID(path.ID);
            }

            if (sink.IsAttached)
            {
                var path = pipeline.PathCollection.Cast<IDTSPath100>().First(item => item.EndPoint.ID == sink.ID);
                pipeline.PathCollection.RemoveObjectByID(path.ID);
            }

            // Add Source Path
            var sourcePath = pipeline.PathCollection.New();
            var newFlowletStart = pipeline.GetObjectByID(_flowletIdMapping[flowletStart.ID]) as IDTSInput100;
            sourcePath.AttachPathAndPropagateNotifications(source, newFlowletStart);

            foreach (IDTSPath100 path in flowlet.PathCollection)
            {
                ClonePath(pipeline, path);
            }

            // Add Sink Path
            var sinkPath = pipeline.PathCollection.New();
            var newFlowletEnd = pipeline.GetObjectByID(_flowletIdMapping[flowletEnd.ID]) as IDTSOutput100;
            sinkPath.AttachPathAndPropagateNotifications(newFlowletEnd, sink);

            return package;
        }

        private IDTSInput100 FindStart(MainPipe flowlet, IDTSOutput100 source, Dictionary<string, string> mapping)
        {
            IDTSInput100 startInput = null;

            var components = FindMatchingComponents(flowlet, ManagedComponentId, DummySourceTypeName).ToList();

            // May eventually support multiples, but for now, throw an error
            if (components.Count() != 1)
            {
                throw new ArgumentException("The flowlet can have one and only one Placeholder Source component.", "flowlet");
            }

            foreach (var component in components)
            {
                // Create ID Mappings
                // TODO: Hardcoding the output, since PlaceHolder source only has one. Might need to update at some point
                IDTSOutput100 output = component.OutputCollection[0];
                foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                {
                    string targetMapping;
                    if (mapping.TryGetValue(outputColumn.IdentificationString, out targetMapping))
                    {
                        _flowletIdMapping.Add(outputColumn.ID, FindColumn(source.OutputColumnCollection, targetMapping).ID);
                    }
                }

                int pathId = 0;
                // Get Input
                foreach (IDTSPath100 path in flowlet.PathCollection)
                {
                    if (path.StartPoint.ID == output.ID)
                    {
                        startInput = path.EndPoint;
                        pathId = path.ID;
                    }
                }

                // Remove Component
                if (pathId > 0)
                {
                    flowlet.PathCollection.RemoveObjectByID(pathId);
                }

                flowlet.ComponentMetaDataCollection.RemoveObjectByID(component.ID);
            }

            return startInput;
        }

        private IDTSOutput100 FindEnd(MainPipe flowlet, IDTSInput100 sink, Dictionary<string, string> mapping)
        {
            IDTSOutput100 endOutput = null;

            var components = FindMatchingComponents(flowlet, ManagedComponentId, DummyTargetName).ToList();

            // May eventually support multiples, but for now, throw an error
            if (components.Count() != 1)
            {
                throw new ArgumentException("The flowlet can have one and only one Placeholder Target component.", "flowlet");
            }

            foreach (var component in components)
            {
                // Create ID Mappings
                // TODO: Hardcoding the input, since PlaceHolder Target only has one. Might need to update at some point
                IDTSInput100 input = component.InputCollection[0];
                foreach (IDTSInputColumn100 outputColumn in input.InputColumnCollection)
                {
                    string targetMapping;
                    if (mapping.TryGetValue(outputColumn.IdentificationString, out targetMapping))
                    {
                        _flowletIdMapping.Add(outputColumn.ID, FindColumn(sink.InputColumnCollection, targetMapping).ID);
                    }
                }

                int pathId = 0;
                // Get Input
                foreach (IDTSPath100 path in flowlet.PathCollection)
                {
                    if (path.EndPoint.ID == input.ID)
                    {
                        endOutput = path.StartPoint;
                        pathId = path.ID;
                    }
                }

                // Remove Component
                if (pathId > 0)
                {
                    flowlet.PathCollection.RemoveObjectByID(pathId);
                }

                flowlet.ComponentMetaDataCollection.RemoveObjectByID(component.ID);
            }

            return endOutput;
        }

        private IDTSObject100 FindColumn(IDTSOutputColumnCollection100 columnCollection, string columnId)
        {
            foreach (IDTSObject100 column in columnCollection)
            {
                if (column.IdentificationString == columnId)
                {
                    return column;
                }
            }

            return null;
        }

        private IDTSObject100 FindColumn(IDTSInputColumnCollection100 columnCollection, string columnId)
        {
            foreach (IDTSObject100 column in columnCollection)
            {
                if (column.IdentificationString == columnId)
                {
                    return column;
                }
            }

            return null;
        }

        private IEnumerable<IDTSComponentMetaData100> FindMatchingComponents(MainPipe flowlet, string componentClassId, string userComponentTypeName = "", bool partialMatch = true)
        {
            for (int i = 0; i < flowlet.ComponentMetaDataCollection.Count; i++)
            {
                IDTSComponentMetaData100 component = flowlet.ComponentMetaDataCollection[i];
                if (component.ComponentClassID == componentClassId)
                {
                    if (string.IsNullOrEmpty(userComponentTypeName))
                    {
                        yield return component;
                    }

                    string typeName = component.CustomPropertyCollection["UserComponentTypeName"].Value.ToString();

                    if (userComponentTypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return component;
                    }
                    else if (partialMatch && typeName.StartsWith(userComponentTypeName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return component;
                    }
                }
            }
        }

        private int GetIdForRefId(MainPipe pipeline, string refId)
        {
            // @"Derived Column.Inputs[Derived Column Input].Columns[Name]"
            var refIdParts = refId.Split(new string[] { @"." }, StringSplitOptions.None);

            string refIdPart = refIdParts[0];
            string refIdPart2 = GetIndexValue(refIdParts[1]);
            string refIdPart3 = GetIndexValue(refIdParts[2]);

            return pipeline.ComponentMetaDataCollection[refIdPart].InputCollection[refIdPart2].InputColumnCollection[refIdPart3].ID;
        }

        private string GetIndexValue(string refIdPart)
        {
            int startIndex = refIdPart.IndexOf("[");
            int endIndex = refIdPart.IndexOf("]");
            return refIdPart.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        private void ClonePath(MainPipe pipeline, IDTSPath100 sourcePath)
        {
            IDTSPath100 target = pipeline.PathCollection.New();
            _flowletIdMapping.Add(sourcePath.ID, target.ID);

            target.Description = sourcePath.Description;
            target.Name = sourcePath.Name;

            var output = pipeline.GetObjectByID(_flowletIdMapping[sourcePath.StartPoint.ID]) as IDTSOutput100;
            var input = pipeline.GetObjectByID(_flowletIdMapping[sourcePath.EndPoint.ID]) as IDTSInput100;
            target.AttachPathAndPropagateNotifications(output, input);
        }

        private void CloneComponent(MainPipe pipeline, IDTSComponentMetaData100 sourceComponent)
        {
            IDTSComponentMetaData100 targetComponent = pipeline.ComponentMetaDataCollection.New();
            _flowletIdMapping.Add(sourceComponent.ID, targetComponent.ID);

            targetComponent.ComponentClassID = sourceComponent.ComponentClassID;

            targetComponent.ContactInfo = sourceComponent.ContactInfo;
            targetComponent.Description = sourceComponent.Description;
            if (!sourceComponent.IsDefaultLocale)
            {
                targetComponent.LocaleID = sourceComponent.LocaleID;
            }
            targetComponent.Name = sourceComponent.Name;
            targetComponent.PipelineVersion = sourceComponent.PipelineVersion;
            targetComponent.UsesDispositions = sourceComponent.UsesDispositions;
            targetComponent.ValidateExternalMetadata = sourceComponent.ValidateExternalMetadata;
            targetComponent.Version = sourceComponent.Version;

            CopyCustomPropertyCollection(sourceComponent.CustomPropertyCollection, targetComponent.CustomPropertyCollection);
            CopyInputCollection(sourceComponent.InputCollection, targetComponent.InputCollection);
            CopyOutputCollection(sourceComponent.OutputCollection, targetComponent.OutputCollection);
            CopyConnectionCollection(sourceComponent.RuntimeConnectionCollection, targetComponent.RuntimeConnectionCollection);
        }

        private void CopyConnectionCollection(IDTSRuntimeConnectionCollection100 sourceConnections, IDTSRuntimeConnectionCollection100 targetConnections)
        {
            foreach (IDTSRuntimeConnection100 sourceConnection in sourceConnections)
            {
                var targetConnection = targetConnections.New();
                targetConnection.ConnectionManager = sourceConnection.ConnectionManager; // TODO: Add code to match up / add the connection manager
                targetConnection.ConnectionManagerID = sourceConnection.ConnectionManagerID; // TODO: Handle matching ID
                targetConnection.Description = sourceConnection.Description;
                targetConnection.Name = sourceConnection.Name;
            }
        }

        private void CopyOutputCollection(IDTSOutputCollection100 sourceOutputs, IDTSOutputCollection100 targetOutputs)
        {
            foreach (IDTSOutput100 source in sourceOutputs)
            {
                var target = targetOutputs.New();
                _flowletIdMapping.Add(source.ID, target.ID);
                target.SynchronousInputID = _flowletIdMapping[source.SynchronousInputID];
                target.Dangling = source.Dangling;
                target.DeleteOutputOnPathDetached = source.DeleteOutputOnPathDetached;
                target.Description = source.Description;
                target.ErrorOrTruncationOperation = source.ErrorOrTruncationOperation;
                target.ErrorRowDisposition = source.ErrorRowDisposition;
                target.ExclusionGroup = source.ExclusionGroup;
                target.HasSideEffects = source.HasSideEffects;
                target.IsErrorOut = source.IsErrorOut;
                target.Name = source.Name;
                target.TruncationRowDisposition = source.TruncationRowDisposition;

                if (target.SynchronousInputID == 0)
                {
                    target.IsSorted = source.IsSorted;
                }

                CopyExternalMetadataColumnCollection(source.ExternalMetadataColumnCollection, target.ExternalMetadataColumnCollection);
                CopyOutputColumnCollection(source.OutputColumnCollection, target.OutputColumnCollection, target.IsSorted);
                CopyCustomPropertyCollection(source.CustomPropertyCollection, target.CustomPropertyCollection);
            }
        }

        private void CopyInputCollection(IDTSInputCollection100 sourceInputs, IDTSInputCollection100 targetInputs)
        {
            foreach (IDTSInput100 source in sourceInputs)
            {
                var target = targetInputs.New();
                _flowletIdMapping.Add(source.ID, target.ID);

                target.Dangling = source.Dangling;
                target.Description = source.Description;
                target.ErrorOrTruncationOperation = source.ErrorOrTruncationOperation;
                target.ErrorRowDisposition = source.ErrorRowDisposition;
                target.HasSideEffects = source.HasSideEffects;
                target.Name = source.Name;
                target.TruncationRowDisposition = source.TruncationRowDisposition;

                CopyExternalMetadataColumnCollection(source.ExternalMetadataColumnCollection, target.ExternalMetadataColumnCollection);
                CopyInputColumnCollection(source.InputColumnCollection, target.InputColumnCollection);
                CopyCustomPropertyCollection(source.CustomPropertyCollection, target.CustomPropertyCollection);
            }
        }

        private void CopyOutputColumnCollection(IDTSOutputColumnCollection100 sourceCollection, IDTSOutputColumnCollection100 targetCollection, bool sourceOutput)
        {
            foreach (IDTSOutputColumn100 source in sourceCollection)
            {
                IDTSOutputColumn100 target = null;

                if (source.SpecialFlags > 0)
                {
                    foreach (IDTSOutputColumn100 match in targetCollection)
                    {
                        if (match.SpecialFlags == source.SpecialFlags)
                        {
                            target = match;
                            break;
                        }
                    }
                }
                else
                {
                    target = targetCollection.New();
                }

                if (target == null)
                {
                    throw new ArgumentException("Source Collection was an Error output, but couldn't find error columns in target collection", "targetCollection");
                }

                _flowletIdMapping.Add(source.ID, target.ID);

                target.Description = source.Description;
                target.ErrorOrTruncationOperation = source.ErrorOrTruncationOperation;
                target.ErrorRowDisposition = source.ErrorRowDisposition;
                target.ExternalMetadataColumnID = _flowletIdMapping[source.ExternalMetadataColumnID];
                target.LineageID = _flowletIdMapping[source.LineageID];
                target.MappedColumnID = _flowletIdMapping[source.MappedColumnID];
                target.Name = source.Name;

                target.TruncationRowDisposition = source.TruncationRowDisposition;

                if (sourceOutput)
                {
                    target.ComparisonFlags = source.ComparisonFlags;
                    target.SortKeyPosition = source.SortKeyPosition;
                }

                target.SetDataTypeProperties(source.DataType, source.Length, source.Precision, source.Scale, source.CodePage);
                CopyCustomPropertyCollection(source.CustomPropertyCollection, target.CustomPropertyCollection);
            }
        }

        private void CopyExternalMetadataColumnCollection(IDTSExternalMetadataColumnCollection100 sourceCollection, IDTSExternalMetadataColumnCollection100 targetCollection)
        {
            foreach (IDTSExternalMetadataColumn100 source in sourceCollection)
            {
                var target = targetCollection.New();
                _flowletIdMapping.Add(source.ID, target.ID);

                target.CodePage = source.CodePage;
                target.DataType = source.DataType;
                target.Description = source.Description;
                target.Length = source.Length;
                target.MappedColumnID = _flowletIdMapping[source.MappedColumnID];
                target.Name = source.Name;
                target.Precision = source.Precision;
                target.Scale = source.Scale;

                CopyCustomPropertyCollection(source.CustomPropertyCollection, target.CustomPropertyCollection);
            }
        }

        private void CopyInputColumnCollection(IDTSInputColumnCollection100 sourceCollection, IDTSInputColumnCollection100 targetCollection)
        {
            foreach (IDTSInputColumn100 source in sourceCollection)
            {
                var target = targetCollection.New();
                if (!_flowletIdMapping.ContainsKey(source.ID))
                {
                    // May have been added during the source column mapping
                    _flowletIdMapping.Add(source.ID, target.ID);
                }

                target.Description = source.Description;
                target.ErrorOrTruncationOperation = source.ErrorOrTruncationOperation;
                target.ErrorRowDisposition = source.ErrorRowDisposition;
                target.ExternalMetadataColumnID = _flowletIdMapping[source.ExternalMetadataColumnID];
                target.LineageID = _flowletIdMapping[source.LineageID];
                target.MappedColumnID = _flowletIdMapping[source.MappedColumnID];
                ////target.Name = source.Name;  // Seems that names aren't necessary on input columns - setting it forces a name to be output in the XML
                target.TruncationRowDisposition = source.TruncationRowDisposition;
                target.UsageType = source.UsageType;

                CopyCustomPropertyCollection(source.CustomPropertyCollection, target.CustomPropertyCollection);
            }
        }

        private void CopyCustomPropertyCollection(IDTSCustomPropertyCollection100 sourcePropertyCollection, IDTSCustomPropertyCollection100 targetPropertyCollection)
        {
            foreach (IDTSCustomProperty100 source in sourcePropertyCollection)
            {
                IDTSCustomProperty100 target = targetPropertyCollection.New();
                _flowletIdMapping.Add(source.ID, target.ID);
                target.Name = source.Name;
                target.ContainsID = source.ContainsID;  // TODO: Handle IDs - need to search property value. Possible Regex: @"[" + IdPrefix + @"][0-9]+"; IdPrefix = #
                target.Description = source.Description;
                target.EncryptionRequired = source.EncryptionRequired;
                target.ExpressionType = source.ExpressionType;
                target.State = source.State;
                target.TypeConverter = source.TypeConverter;
                target.UITypeEditor = source.UITypeEditor;
                target.Value = source.ContainsID ? ReplaceIds(source.Value.ToString()) : source.Value;
            }
        }

        private string ReplaceIds(string value)
        {
            var idMatch = new Regex(@"[#][0-9]+");
            var evaluator = new MatchEvaluator(ReplaceId);

            return idMatch.Replace(value, evaluator);
        }

        private string ReplaceId(Match match)
        {
            int id = Convert.ToInt32(match.Value.Replace("#", string.Empty));
            int newId;
            if (!_flowletIdMapping.TryGetValue(id, out newId))
            {
                if (!_sourceColumnMappings.TryGetValue(id, out newId))
                {
                    newId = id;
                }
            }

            return string.Format("#{0}", newId);
        }

        private class ComponentEventHandler : IDTSComponentEvents
        {
            public void FireWarning(int warningCode, string subComponent, string description, string helpFile, int helpContext)
            {
                Debug.WriteLine("[Warning] {0}: {1}", subComponent, description);
            }

            public void FireInformation(int informationCode, string subComponent, string description, string helpFile, int helpContext, ref bool fireAgain)
            {
                fireAgain = false;
            }

            public bool FireError(int errorCode, string subComponent, string description, string helpFile, int helpContext)
            {
                Debug.WriteLine("[Error] {0}: {1}", subComponent, description);
                return true;
            }

            public bool FireQueryCancel()
            {
                return false;
            }

            public void FireBreakpointHit(BreakpointTarget breakpointTarget)
            {
            }

            public void FireProgress(string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh, string subComponent, ref bool fireAgain)
            {
            }

            public void FireCustomEvent(string eventName, string eventText, ref object[] arguments, string subComponent, ref bool fireAgain)
            {
                Debug.WriteLine("[Custom] {0}: {1} - {2}", subComponent, eventName, eventText);
            }
        }
    }
}
