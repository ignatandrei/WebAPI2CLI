using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace CLIExecute
{
    class EnumerateWebAPI
    {
        private readonly ICollection<string> addresses;
        private readonly IApiDescriptionGroupCollectionProvider api;
        
        public EnumerateWebAPI(ICollection<string> addresses, IApiDescriptionGroupCollectionProvider api)
        {
            this.addresses = addresses;
            this.api = api;
            
        }
        private ListOfBlockly FindWebAPI()
        {
            var allCommands = new ListOfBlockly();

            var allAdresses = addresses.ToArray();


            var groups = api.ApiDescriptionGroups;

            foreach (var g in groups.Items)
            {

                foreach (var api in g.Items)
                {

                    foreach (var adress in allAdresses)
                    {
                        var ad = new Uri(adress);
                        var v1 = new BlocklyGenerator();
                        v1.NameCommand = api.RelativePath;
                        v1.Host = ad.GetLeftPart(UriPartial.Scheme);
                        v1.RelativeRequestUrl = api.RelativePath;
                        v1.Verb = api.HttpMethod;
                        v1.Params = GetParameters(api.ParameterDescriptions.ToArray());
                        allCommands.Add(v1);
                    }

                }
            }
            return allCommands;
        }
        Dictionary<string, (Type type, BindingSource bs)> GetParameters(ApiParameterDescription[] parameterDescriptions)
        {
            if (parameterDescriptions?.Length == 0)
                return null;

            var desc = new Dictionary<string, (Type type, BindingSource bs) >();
            var pdAll = parameterDescriptions
                .Where(it => it != null)
                .Select(it => it.ParameterDescriptor)
                .Where(it => it != null)
                .Distinct()
                .ToArray();
            var strType = typeof(string).FullName;
            var okBindingSource = new[]
            {
                BindingSource.Body,
                BindingSource.Form,
                BindingSource.Path,
                BindingSource.Query
            };

            foreach (var pd in pdAll)
            {
                if( okBindingSource.Contains(pd.BindingInfo?.BindingSource) )
                    desc.Add(pd.Name,  (pd.ParameterType, pd.BindingInfo.BindingSource));
            }
            return desc;
        }
        

    }
}
