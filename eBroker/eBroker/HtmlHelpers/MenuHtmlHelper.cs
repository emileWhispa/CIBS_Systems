using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eBroker;

namespace eBroker
{
    public static class MenuHtmlHelper
    {
        public static MenuDisplay BmatMenu(this HtmlHelper helper, List<Menu> menus, bool addAccountMenu = true, int userId = 0)
        {
            return new MenuDisplay(menus, addAccountMenu, helper, userId);
        }
    }

    public class MenuDisplay : IHtmlString
    {
        private readonly List<Menu> _menus;
        private readonly bool addAccMenu;
        private readonly HtmlHelper htmlHelper;
        private readonly int UserId;
        public MenuDisplay(List<Menu> menus, bool addAccountMenu, HtmlHelper helper, int userId)
        {
            _menus = menus;
            addAccMenu = addAccountMenu;
            htmlHelper = helper;
            UserId = userId;
        }
        //Render HTML
        public override string ToString()
        {
            return RenderMenu();
        }

        //Return ToString
        public string ToHtmlString()
        {
            return ToString();
        }

        private string RenderMenu()
        {
            //--- Create main ul
            TagBuilder menu = new TagBuilder("ul");
            menu.Attributes.Add("id", "menu-bar");

            //----Get main menus
            var menus1 = _menus.Where(x => x.MenuLevel == 0).ToList();
            foreach (var mn1 in menus1)
            {
                TagBuilder menu_main = new TagBuilder("li");
                TagBuilder menu_link = new TagBuilder("a");
                menu_link.Attributes.Add("href", mn1.Url);
                menu_link.SetInnerText(mn1.MenuName);
                menu_main.InnerHtml = menu_link.ToString();
                //--- Sub menus
                var submenus = _menus.Where(x => x.MenuLevel != 0 && x.ParentId == mn1.Id).ToList();
                if (submenus.Count > 0)
                {
                    TagBuilder sub_menus = new TagBuilder("ul");
                    foreach (var smn1 in submenus)
                    {
                        TagBuilder smenu_main = new TagBuilder("li");
                        TagBuilder smenu_link = new TagBuilder("a");

                        if (smn1.Url.Equals("#"))
                            smenu_link.Attributes.Add("href", smn1.Url);
                        else
                        {
                            string[] arrUrl = smn1.Url.Split('/');
                            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                            string url = urlHelper.Action(arrUrl[1], arrUrl[0]);
                            smenu_link.Attributes.Add("href", url);
                        }
                        smenu_link.SetInnerText(smn1.MenuName);
                        smenu_main.InnerHtml = smenu_link.ToString();

                        sub_menus.InnerHtml += smenu_main.ToString();
                    }
                    menu_main.InnerHtml += sub_menus.ToString();
                }
                menu.InnerHtml += menu_main.ToString();
            }
            //---- Add default account menu
            //if (addAccMenu)
            //{
            //    TagBuilder account = new TagBuilder("li");
            //    account.InnerHtml = "<a href='#'>Account</a>";

            //    TagBuilder acc_subs = new TagBuilder("ul");

            //    TagBuilder logout = new TagBuilder("li");
            //    TagBuilder link = new TagBuilder("a");
            //    var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            //    string url = urlHelper.Action("Logout", "Account");
            //    link.Attributes.Add("href", url);
            //    link.SetInnerText("Log Out");
            //    logout.InnerHtml = link.ToString();

            //    acc_subs.InnerHtml += logout.ToString();

            //    TagBuilder changepass = new TagBuilder("li");
            //    link = new TagBuilder("a");
            //    url = urlHelper.Action("ChangePass", "Account", new { userId = UserId });
            //    link.Attributes.Add("href", url);
            //    link.SetInnerText("Change Password");

            //    changepass.InnerHtml = link.ToString();
            //    acc_subs.InnerHtml += changepass.ToString();

            //    account.InnerHtml += acc_subs.ToString();

            //    menu.InnerHtml += account.ToString();
            //}
            return menu.ToString();
        }
    }
    //public static class MenuHtmlHelper
    //{
    //    public static MenuDisplay BmatMenu(this HtmlHelper helper, List<Menu> menus, bool addAccountMenu = true)
    //    {
    //        return new MenuDisplay(menus, addAccountMenu);
    //    }
    //}

    //public class MenuDisplay : IHtmlString
    //{
    //    private readonly List<Menu> _menus;
    //    private readonly bool addAccMenu;
    //    public MenuDisplay(List<Menu> menus, bool addAccountMenu)
    //    {
    //        _menus = menus;
    //        addAccMenu = addAccountMenu;
    //    }
    //    //Render HTML
    //    public override string ToString()
    //    {
    //        return RenderMenu();
    //    }

    //    //Return ToString
    //    public string ToHtmlString()
    //    {
    //        return ToString();
    //    }

    //    private string RenderMenu()
    //    {
    //        //--- Create main ul
    //        TagBuilder menu = new TagBuilder("ul");
    //        menu.AddCssClass("menu");
    //        //----Get main menus
    //        var menus1 = _menus.Where(x => x.MenuLevel == 0).ToList();
    //        foreach (var mn1 in menus1)
    //        {
    //            TagBuilder menu_main = new TagBuilder("li");
    //            TagBuilder menu_link = new TagBuilder("a");
    //            menu_link.Attributes.Add("href", mn1.Url);
    //            menu_link.SetInnerText(mn1.MenuName);
    //            menu_main.InnerHtml = menu_link.ToString();
    //            //--- Sub menus
    //            var submenus = _menus.Where(x => x.MenuLevel != 0 && x.ParentId == mn1.Id).ToList();
    //            if (submenus.Count > 0)
    //            {
    //                TagBuilder sub_menus = new TagBuilder("ul");
    //                foreach (var smn1 in submenus)
    //                {
    //                    TagBuilder smenu_main = new TagBuilder("li");
    //                    TagBuilder smenu_link = new TagBuilder("a");
    //                    smenu_link.Attributes.Add("href", smn1.Url);
    //                    smenu_link.SetInnerText(smn1.MenuName);
    //                    smenu_main.InnerHtml = smenu_link.ToString();

    //                    sub_menus.InnerHtml += smenu_main.ToString();
    //                }
    //                menu_main.InnerHtml += sub_menus.ToString();
    //            }
    //            menu.InnerHtml += menu_main.ToString();
    //        }
    //        //---- Add default account menu
    //        if (addAccMenu)
    //        {
    //            TagBuilder account = new TagBuilder("li");
    //            account.InnerHtml = "<a href='#'>Account</a>";
    //            TagBuilder acc_subs = new TagBuilder("ul");

    //            TagBuilder logout = new TagBuilder("li");
    //            logout.InnerHtml = "<a href='/Account/Logout'>Log Out</a>";
    //            acc_subs.InnerHtml += logout.ToString();


    //        }
    //        return menu.ToString();
    //    }
    //}

}