﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace WorkStatus
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
        }

        private void Do_Render_CWB_Data()
        {
            if (Page.IsPostBack == false)
            {
                WebClient webClient = new WebClient
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                string json = webClient.DownloadString("" +
                    "https://opendata.cwb.gov.tw/api/v1/rest/datastore/F-D0047-089?Authorization=rdec-key-123-45678-011121314&elementName=Wx,T,AT,PoP6h");

                Session["V4CWBJson"] = json;
                CWBWeatherApiClass data = Newtonsoft.Json.JsonConvert.DeserializeObject<CWBWeatherApiClass>(json);
                DropDownList1.Items.Add(new System.Web.UI.WebControls.ListItem(Properties.LanguageResource.Select_Region, "-1", true));
                for (int i = 0; i < data.records.locations[0].location.Length; i++)
                {
                    System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem(data.records.locations[0].location[i].locationName, i.ToString());
                    DropDownList1.Items.Add(listItem);
                }
                webClient.Dispose();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WebClient webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };

            #region Data update

            if (Convert.ToInt32(Application["V4UnixTimeUpdate"].ToString()) < (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)
            {
                Application["V4UnixTimeUpdate"] = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 1800;
                //html的node用大寫
                string html = webClient.DownloadString("https://www.dgpa.gov.tw/typh/daily/nds.html").Replace("<FONT", "<SPAN").Replace("</FONT>", "</SPAN>");
                string table = html.Substring(html.IndexOf("<TABLE"), html.IndexOf("</TABLE>") - html.IndexOf("<TABLE") + 8);
                //xmlDocument.LoadXml(webClient.DownloadString("https://www.dgpa.gov.tw/typh/daily/nds.html"));
                //HtmlWeb webClient = new HtmlWeb();
                Application["V4HtmlTable"] = table;
                html = webClient.DownloadString("https://www.dgpa.gov.tw/typh/daily/ndse.html").Replace("<FONT", "<SPAN").Replace("</FONT>", "</SPAN>");
                table = html.Substring(html.IndexOf("<TABLE"), html.IndexOf("</TABLE>") - html.IndexOf("<TABLE") + 8);
                Application["EV4HtmlTable"] = table;
            }

            #endregion Data update

            if (Session["V4Verify"].ToString() == "true")
            {
                #region Script region

                System.Web.UI.HtmlControls.HtmlGenericControl htmlGenericControl = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
                htmlGenericControl.Attributes["type"] = "text/javascript";
                //htmlGenericControl.Attributes["defer"] = "defer";
                // htmlGenericControl.Attributes["async"] = "async";
                System.Web.UI.HtmlControls.HtmlGenericControl ImproveJS = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
                System.Web.UI.HtmlControls.HtmlGenericControl ThemeJS = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
                System.Web.UI.HtmlControls.HtmlGenericControl ImproveJSi18n = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
                ImproveJSi18n.Attributes["defer"] = "defer";
                //ImproveJSi18n.Attributes["async"] = "async";
                System.Web.UI.HtmlControls.HtmlGenericControl css = new System.Web.UI.HtmlControls.HtmlGenericControl("link");
                css.Attributes["rel"] = "stylesheet";

                #region Improve

                string u = Request.ServerVariables["HTTP_USER_AGENT"];
                Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex s = new Regex(@"/Mobile|iP(hone|od|ad)|Android|BlackBerry|IEMobile|Kindle|NetFront|Silk-Accelerated|(hpw|web)OS|Fennec|Minimo|Opera M(obi|ini)|Blazer|Dolfin|Dolphin|Skyfire|Zune/", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4)) || s.IsMatch(u)))
                {
                    ImproveJS.Attributes["src"] = "Improve_Table_Mobile.js";
                    MultiView1.ActiveViewIndex = 0;
                }
                else
                {
                    ImproveJS.Attributes["src"] = "Improve_Table.js";
                }
                Page.Header.Controls.Add(ImproveJS);

                #endregion Improve

                #region Improve i18n

                if (Session["L"].ToString() == "C")
                {
                    ImproveJSi18n.Attributes["src"] = "Improve_zh.js";
                }
                else
                {
                    ImproveJSi18n.Attributes["src"] = "Improve_en.js";
                }

                if (Session["L"].ToString() == "C")
                {
                    css.Attributes["href"] = "Improve_Table_zh.css";
                    ImproveJSi18n.Attributes["src"] = "Improve_zh.js";
                    Label_1.Text = Application["V4HtmlTable"].ToString();
                }
                else
                {
                    Label_1.Text = Application["EV4HtmlTable"].ToString();
                    css.Attributes["href"] = "Improve_Table_en.css";
                    ImproveJSi18n.Attributes["src"] = "Improve_en.js";
                }
                Page.Header.Controls.Add(ImproveJSi18n);
                Page.Header.Controls.Add(css);

                #endregion Improve i18n

                #region Theme

                //==========================================

                if (Request.Cookies["V4theme"] == null)
                {
                    Response.Cookies["V4theme"].Value = "light";

                    ThemeJS.Attributes["src"] = "Improve_Table_Light.js";
                    Page.Header.Controls.Add(ThemeJS);
                }

                //==========================================

                if (Request.QueryString["changetheme"] == "true")
                {
                    if (Request.Cookies["V4theme"].Value == "light")
                    {
                        Response.Cookies["V4theme"].Value = "dark";
                        Response.Cookies["V4theme"].Expires = DateTime.UtcNow.AddYears(999);
                    }
                    else
                    {
                        Response.Cookies["V4theme"].Value = "light";
                        Response.Cookies["V4theme"].Expires = DateTime.UtcNow.AddYears(999);
                    }

                    Response.Redirect("./");
                }
                else if (Request.Cookies["V4theme"].Value == "dark")
                {
                    ThemeJS.Attributes["src"] = "Improve_Table_Dark.js";
                }
                else
                {
                    ThemeJS.Attributes["src"] = "Improve_Table_Light.js";
                }
                Page.Header.Controls.Add(ThemeJS);

                #endregion Theme

                #endregion Script region

                #region Dispose

                ImproveJS.Dispose();
                ThemeJS.Dispose();
                ImproveJSi18n.Dispose();
                webClient.Dispose();

                #endregion Dispose
            }

            #region reCaptchaV3

            if (Request.QueryString["token"] != null)
            {
                var responseString = webClient.DownloadString(
                    "https://www.google.com/recaptcha/api/siteverify?secret=6Lc_o3cUAAAAAE0veTRNuvENGiaU-QNTmjFQCok6&response=" + Request.QueryString["token"]);
                reCaptchaV3Response reCaptchaV3Response = Newtonsoft.Json.JsonConvert.DeserializeObject<reCaptchaV3Response>(responseString);
                if (reCaptchaV3Response.success == true & reCaptchaV3Response.score >= 0.7)
                {
                    Session["V4Verify"] = "true";
                    Response.Redirect("./");
                }
                else if (Session["V4Verify"].ToString() != "true")
                {
                    Response.Redirect("./Default?notverify=true");
                }
            }
            else if (Session["V4Verify"].ToString() == "true")
            {
                Do_Render_CWB_Data();
            }
            else if (Session["V4Verify"].ToString() != "true" & Request.QueryString["notverify"] == null)
            {
                Response.Redirect("./?notverify=true");
            }

            #endregion reCaptchaV3
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.DropDownList dropDownList = (System.Web.UI.WebControls.DropDownList)sender;
            CWBWeatherApiClass data = Newtonsoft.Json.JsonConvert.DeserializeObject<CWBWeatherApiClass>(Session["V4CWBJson"].ToString());
            int index = int.Parse(dropDownList.SelectedValue);
            if (index == -1)
            {
                ChartCWBDiv.Visible = false;
                Label2.Text = "";
                return;
            }
            string TimeLabel = "";
            string TempLabel = "";
            string TempApparentLabel = "";
            string RainLabel = "";
            List<int> list = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                //0:WX,AT ,T ,PoP6H
                int int1 = int.Parse(data.records.locations[0].location[index].weatherElement[3].time[i].elementValue[0].value);
                int int2 = int.Parse(data.records.locations[0].location[index].weatherElement[3].time[i + 1].elementValue[0].value);
                if (i == 0)
                {
                    list.Add(int1);
                    list.Add((int1 + int2) / 2);
                    list.Add(int2);
                    //RainLabel += int1 +","+ (int1 + int2) / 2+"," + int2;
                }
                else
                {
                    list.Add((int1 + int2) / 2);
                    list.Add(int2);
                    //RainLabel += int1 + "," + (int1 + int2) / 2 + "," + int2;
                }
            }
            for (int i = 0; i < 9; i++)
            {
                //0:WX,AT ,T ,PoP6H
                string time = data.records.locations[0].location[index].weatherElement[2].time[i].dataTime.ToString();
                if (i == 8)
                {
                    //TimeLabel += " ' " + time.Split(' ')[0].Split('-')[1] + "/" + time.Split(' ')[0].Split('-')[2] + " " + time.Split(' ')[1].Split(':')[0] + ":" + time.Split(' ')[1].Split(':')[1] + " | 💧 " + list[i] + "%' ";
                    TimeLabel += " '" + time.Split(' ')[1].Split(':')[0] + " |💧" + list[i] + "%' ";
                    TempApparentLabel += " ' " + data.records.locations[0].location[index].weatherElement[1].time[i].elementValue[0].value + " ' ";
                    TempLabel += " ' " + data.records.locations[0].location[index].weatherElement[2].time[i].elementValue[0].value + " ' ";
                }
                else
                {
                    //TimeLabel += " ' " + time.Split(' ')[0].Split('-')[1] + "/" + time.Split(' ')[0].Split('-')[2] + " " + time.Split(' ')[1].Split(':')[0] + ":" + time.Split(' ')[1].Split(':')[1] + " | 💧 " + list[i] + "%' " + ",";
                    TimeLabel += " '" + time.Split(' ')[1].Split(':')[0] + " |💧" + list[i] + "%' " + ",";
                    TempApparentLabel += " ' " + data.records.locations[0].location[index].weatherElement[1].time[i].elementValue[0].value + " ' " + ",";
                    TempLabel += " ' " + data.records.locations[0].location[index].weatherElement[2].time[i].elementValue[0].value + " ' " + ",";
                }
            }
            foreach (var item in list)
            {
                RainLabel += "'" + item + "',";
            }
            RainLabel = RainLabel.Remove(RainLabel.Length - 1);
            //體感溫度
            ChartCWBDiv.Visible = true;
            Label2.Text = @" <script>
               new Chart(document.getElementById('ChartCWB'), {
                   type: 'line',
                   data: {
                       labels: [" + TimeLabel + @"],
                       datasets: [
                        {
                           data: [" + TempLabel + @"],
                           label: 'Temp(℃)',
                           borderColor: '#007bff',
                           fill: false
                       },
                       {
                           data: [" + TempApparentLabel + @"],
                           label: 'Apparent Temp(℃)',
                           borderColor: '#17a2b8',
                           fill: false
                       }]
                   },
                   options: {
                       title: {
                           display: true,
                           text: 'Weather(24H)'
                       },
                             maintainAspectRatio: false,
                              responsive: true
                   }
               });
    </script>";
        }
    }
}