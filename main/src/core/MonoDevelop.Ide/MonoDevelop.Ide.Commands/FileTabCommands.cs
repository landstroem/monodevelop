//
// FileTabCommands.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using Gtk;

using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Navigation;
using MonoDevelop.Core;
using System.Linq;

namespace MonoDevelop.Ide.Commands
{
	public enum FileTabCommands
	{
		CloseAll,
		CloseAllButThis,
		CopyPathName,
		ToggleMaximize,
		ReopenClosedTab,
		CloseAllExceptPinned
	}
	
	class CloseAllHandler : CommandHandler
	{
		protected override void Run ()
		{
			var active = IdeApp.Workbench.ActiveDocument;
			if (active == null)
				return;
			var activeNotebook = ((SdiWorkspaceWindow)active.Window).TabControl;
			foreach (Document doc in IdeApp.Workbench.Documents.ToArray ()) {
				var w1 = (SdiWorkspaceWindow) doc.Window;
				if (w1.TabControl == activeNotebook)
					doc.Close ();
			}
		}
	}
	
	class CloseAllButThisHandler : CommandHandler
	{
		protected override void Run ()
		{
			var active = IdeApp.Workbench.ActiveDocument;
			if (active == null)
				return;
			var activeNotebook = ((SdiWorkspaceWindow)active.Window).TabControl;
			foreach (Document doc in IdeApp.Workbench.Documents.ToArray ()) {
				var w1 = (SdiWorkspaceWindow) doc.Window;
				if (w1.TabControl == activeNotebook && doc != active)
					doc.Close ();
			}
		}
	}

	class CloseAllExceptPinnedHandler : CommandHandler
	{
		protected override void Run ()
		{
			var active = IdeApp.Workbench.ActiveDocument;
			if (active == null)
				return;

			var deleteCache = new System.Collections.Generic.List<Document>();
			var w1 = (SdiWorkspaceWindow) active.Window;
			foreach (var item in w1.TabControl.Tabs.Where(s => !s.IsPinned)) {
				var workspaceWindow = item.Content as SdiWorkspaceWindow; 
				if (workspaceWindow != null && workspaceWindow.Document != null)
					deleteCache.Add (workspaceWindow.Document);
			}

			foreach (Document doc in IdeApp.Workbench.Documents.ToArray ()) {
				if (deleteCache.Exists(d => d == doc))
					doc.Close();
			}
		}
	}
	
	class ToggleMaximizeHandler : CommandHandler
	{
		protected override void Run ()
		{
			IdeApp.Workbench.ToggleMaximize ();
		}
	}
	
	class CopyPathNameHandler : CommandHandler
	{
		protected override void Run ()
		{
			Document document = IdeApp.Workbench.ActiveDocument;
			if (document == null)
				return;
			var fileName = document.FileName;
			if (fileName == null)
				return;
			Clipboard clipboard = Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
			clipboard.Text = fileName;
			clipboard = Clipboard.Get (Gdk.Atom.Intern ("PRIMARY", false));
			clipboard.Text = fileName;
		}
	}

	class ReopenClosedTabHandler : CommandHandler
	{
		protected override void Run ()
		{
			NavigationHistoryService.OpenLastClosedDocument ();
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = NavigationHistoryService.HasClosedDocuments;
		}
	}
}
