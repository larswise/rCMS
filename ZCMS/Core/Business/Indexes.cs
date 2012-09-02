using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;
using Raven.Client.Extensions;
using Raven.Abstractions.Indexing;
using ZCMS.Core.Business.Content;

namespace ZCMS.Core.Business
{
    public class PageIndexer : AbstractMultiMapIndexCreationTask<PageIndexer.Result>
    {
        public class Result 
        {
            public string Body { get; set; }
            public string Slug { get; set; }
        }

        public PageIndexer()
        {
            AddMap<ZCMSPage>(items => from x in items
                            select new Result 
                            {  
                                Slug = x.SlugValue,
                                Body = x.PropertyValues + " " + x.WrittenBy + " " + x.LastChangedBy + " " + x.PageName + " " + x.PageID.ToString()
                                 
                            });

            Index(x => x.Body, FieldIndexing.Analyzed);
            Index(x => x.Slug, FieldIndexing.Analyzed);
        }
    }
}