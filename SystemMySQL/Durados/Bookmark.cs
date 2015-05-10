using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Bookmark
    {
        public Bookmark()
        {
            this.Type = 0; //Bookmark
            this.Ordinal = 0; //Order BY Id DESC / ASC
            this.PageNo = 1;
            this.PageSize = 0;
        }

        public Bookmark(int Id, int Type, string Name, int Ordinal)
        {
            this.Type = Type;
            this.Ordinal = Ordinal;

            this.Id = Id;
            this.Name = Name;
            this.PageNo = 1;
            this.PageSize = 0;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Ordinal { get; set; }
        public int Type { get; set; }

        public string Filter { get; set; }


        public string Url { get; set; }
        public string ViewName { get; set; }        
        public string ControllerName { get; set; }
        public string Guid { get; set; }

        private string sortColumn;
        private string sortDirection;
        private int pageNo;
        private int pageSize;

        public string SortColumn
        {
            get
            {
                return sortColumn;
            }
            set
            {
                sortColumn = value;
            }
        }
        public string SortDirection
        {
            get
            {
                return sortDirection;
            }
            set
            {
                sortDirection = value;
            }
        }
        public int PageNo
        {
            get
            {
                if (pageNo < 1) { return 1; }
                else
                {
                    return pageNo;
                }
            }
            set
            {
                pageNo = value;
            }
        }

        public int PageSize
        {
            get
            {
                    return pageSize;
            }
            set
            {
                 pageSize = value;
            }
        }

    }

    public enum BookmarkType
    {
        Bookmark,
        Recent
    }


}
