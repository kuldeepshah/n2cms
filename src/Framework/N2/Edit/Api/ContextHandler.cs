﻿using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace N2.Management.Api
{
	[Service(typeof(IApiHandler))]
	public class ContextHandler : IHttpHandler, IApiHandler
	{
		public ContextHandler()
			: this(Context.Current)
		{
		}

		public ContextHandler(IEngine engine)
		{
			this.engine = engine;
		}

		private IEngine engine;
		private SelectionUtility Selection { get { return engine.RequestContext.HttpContext.GetSelectionUtility(engine); } }

		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(new HttpContextWrapper(context));
		}

		public void ProcessRequest(HttpContextBase context)
		{
			var item = Selection.ParseSelectionFromRequest();

			var selectedUrl = context.Request["selectedUrl"];
			if (item == null && selectedUrl != null)
				item = Selection.ParseUrl(selectedUrl);

			switch (context.Request.PathInfo)
			{
				case "/interface":
					context.Response.WriteJson(engine.Resolve<InterfaceBuilder>().GetInterfaceContextData(context, Selection));
					return;
				case "/full":
					context.Response.WriteJson(new
					{
						Interface = engine.Resolve<InterfaceBuilder>().GetInterfaceContextData(context, Selection),
						Context = engine.Resolve<ContextBuilder>().GetInterfaceContextData(item, selectedUrl)
					});
					return;
				default:
					context.Response.WriteJson(engine.Resolve<ContextBuilder>().GetInterfaceContextData(item, selectedUrl));
					return;
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}