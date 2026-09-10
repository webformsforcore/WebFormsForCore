using System;
using System.Web.UI;

namespace WebFormsForCore.TestApp
{
    public partial class SessionTest : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                StatusLabel.Text = "Page loaded.";
            }

            RefreshLabels();
        }

        protected void IncrementButton_Click(object sender, EventArgs e)
        {
            int count = Session["Counter"] is int i ? i : 0;
            count++;
            Session["Counter"] = count;

            StatusLabel.Text = "Counter incremented to " + count + ".";
            RefreshLabels();
        }

        protected void SaveNoteButton_Click(object sender, EventArgs e)
        {
            Session["Note"] = NoteTextBox.Text;

            StatusLabel.Text = "Note saved to session.";
            RefreshLabels();
        }

        protected void ReloadButton_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "Page reloaded via postback; values below came back from the session store.";
            RefreshLabels();
        }

        protected void AbandonButton_Click(object sender, EventArgs e)
        {
            Session.Abandon();

            StatusLabel.Text = "Session abandoned. Reload the page to confirm the counter and note are gone.";
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            SessionIdLabel.Text = Session.SessionID;
            IsNewSessionLabel.Text = Session.IsNewSession.ToString();
            CounterLabel.Text = (Session["Counter"] ?? 0).ToString();
            NoteLabel.Text = (Session["Note"] as string) ?? "(none)";
        }
    }
}
