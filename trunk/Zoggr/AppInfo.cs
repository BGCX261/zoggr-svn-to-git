namespace Zoggr
{
    using System;
    using System.Reflection;

    public class AppInfo
    {
        public static string GetCompany()
        {
            AssemblyCompanyAttribute customAttribute = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute));
            return customAttribute.Company;
        }

        public static string GetCopyright()
        {
            AssemblyCopyrightAttribute customAttribute = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute));
            return customAttribute.Copyright;
        }

        public static string GetDescription()
        {
            AssemblyDescriptionAttribute customAttribute = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute));
            return customAttribute.Description;
        }

        public static string GetExecutablePath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        public static string GetProduct()
        {
            AssemblyProductAttribute customAttribute = (AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute));
            return customAttribute.Product;
        }

        public static string GetStartupPath()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            return executingAssembly.Location.Substring(0, executingAssembly.Location.LastIndexOf("/"));
        }

        public static string GetTitle()
        {
            AssemblyTitleAttribute customAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            return customAttribute.Title;
        }

        public static string GetTrademark()
        {
            AssemblyTrademarkAttribute customAttribute = (AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTrademarkAttribute));
            return customAttribute.Trademark;
        }

        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
