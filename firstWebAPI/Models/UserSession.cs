using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GivingActuallyAPI.Models.Helper;

namespace GivingActuallyAPI.Models.HelperModels
{
    public class UserSession
    {
        public static string UserName
        {
            get
            {
                if (HttpContext.Current.Session["UserName"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["UserName"].ToString();
            }
            set
            {
                HttpContext.Current.Session["UserName"] = value;
            }
        }

        public static int UserId
        {
            get { return (int)(HttpContext.Current.Session["UserId"]); }
            set { HttpContext.Current.Session["UserId"] = value; }
        }


        public static string FirstLastName
        {
            get
            {
                if (HttpContext.Current.Session["FirstLastName"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["FirstLastName"].ToString();
            }
            set
            {
                HttpContext.Current.Session["FirstLastName"] = value;
            }
        }

        public static bool HasSession
        {
            get
            {
                if (HttpContext.Current.Session["HasSession"] == null)
                    return false;
                else
                    return true;
            }
            set
            {
                HttpContext.Current.Session["HasSession"] = value;
            }
        }
        public static string UserRoleId
        {
            get
            {
                if (HttpContext.Current.Session["UserRoleId"] == null)
                    return "0";
                else
                    return HttpContext.Current.Session["UserRoleId"].ToString();
            }
            set
            {
                HttpContext.Current.Session["UserRoleId"] = value;
            }
        }

        public static RolesEnum? UserRole
        {
            get
            {
                if (HttpContext.Current.Session["UserRole"] == null)
                    return null;
                else
                    return (RolesEnum)Convert.ToInt32(HttpContext.Current.Session["UserRoleId"]);
            }
            set
            {
                HttpContext.Current.Session["UserRole"] = value;
            }
        }


        public static string UserMsg
        {
            get
            {
                if (HttpContext.Current.Session["UserMsg"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["UserMsg"].ToString();
            }
            set
            {
                HttpContext.Current.Session["UserMsg"] = value;
            }
        }
        public static DateTime LastLoginDate
        {
            get
            {
                if (HttpContext.Current.Session["LastLoginDate"] == null)
                    return DateTime.UtcNow;
                else
                    return Convert.ToDateTime(HttpContext.Current.Session["LastLoginDate"]);
            }
            set
            {
                HttpContext.Current.Session["LastLoginDate"] = value;
            }
        }
        public static string UserCountry
        {
            get
            {
                if (HttpContext.Current.Session["UserCountry"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["UserCountry"].ToString();
            }
            set
            {
                HttpContext.Current.Session["UserCountry"] = value;
            }
        }
        public static string isNGO
        {
            get
            {
                if (HttpContext.Current.Session["isNGO"] == null)
                    return null;
                else
                    return (HttpContext.Current.Session["isNGO"]).ToString();
            }
            set
            {
                HttpContext.Current.Session["isNGO"] = value;
            }
        }
        public static string CanEndorse
        {
            get
            {
                if (HttpContext.Current.Session["CanEndorse"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["CanEndorse"].ToString();
            }
            set
            {
                HttpContext.Current.Session["CanEndorse"] = value;
            }
        }
        public static string NGOSector
        {
            get
            {
                if (HttpContext.Current.Session["NGOSector"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["NGOSector"].ToString();
            }
            set
            {
                HttpContext.Current.Session["NGOSector"] = value;
            }
        }

    }
}