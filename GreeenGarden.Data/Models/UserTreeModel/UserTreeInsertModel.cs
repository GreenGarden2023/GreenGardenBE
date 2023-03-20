﻿using System;
namespace GreeenGarden.Data.Models.UserTreeModel
{
	public class UserTreeInsertModel
	{
        public string TreeName { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public List<string> ImgUrls { get; set; }
    }
}

