﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EstrellasDeEsperanza.WebFormsForCore.TestApp
{
	public partial class About : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

        [WebMethod]
        public static void TestMethod()
        {
            HttpContext.Current.Response.Write("ok");
        }
	}
}