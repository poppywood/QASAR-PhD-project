using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Qasar.Controller;
using Qasar.Monitor;

namespace Qasar.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            TestController test = new TestController();
            if (test.LazowskaTest()) Label1.Text = "ok"; else Label1.Text = "fail";
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            TestController test = new TestController();
            if (test.GATest()) Label2.Text = "ok"; else Label2.Text = "fail";
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Controller.Controller c = new Controller.Controller();
            c.CheckCurrentCondition();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            MetricsCollector c = new MetricsCollector();
            //from start date, loop through at 600 seconds gaps until reach enddate
            DateTime start = new DateTime(2008, 06, 23, 08, 00, 00);
            DateTime end = new DateTime(2008, 06, 23, 21, 40, 00);
            DateTime now = start;
            do{
                now = now.AddSeconds(600);
                c.GatherMetrics(600, now);
            }
            while (now < end);

        }
    }
}
