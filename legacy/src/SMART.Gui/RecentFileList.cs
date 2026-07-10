
using System.Collections.Specialized;
using System.Windows;
using SMART.Core;

namespace SMART.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Microsoft.Win32;
    using System.Xml;
    using System.Diagnostics;
    using System.Text;
    using SMART.Gui.Commands;
    using System.Reflection;
    using SMART.Gui.ViewModel;
    using SMART.IOC;
    using System.Linq;
    public interface IPersist
    {
        List<string> RecentFiles(int max);
        void InsertFile(string filepath, int max);
        void RemoveFile(string filepath, int max);
    }
    public class RecentFileList : ObservableCollection<RecentFileViewModel>
    {
        public IPersist Persister { get; set; }

        public void UseRegistryPersister() { Persister = new RegistryPersister(); }
        public void UseRegistryPersister(string key) { Persister = new RegistryPersister(key); }

        public void UseXmlPersister() { Persister = new XmlPersister(); }
        public void UseXmlPersister(string filepath) { Persister = new XmlPersister(filepath); }
        public void UseXmlPersister(Stream stream) { Persister = new XmlPersister(stream); }

        public int MaxNumberOfFiles { get; set; }
        public int MaxPathLength { get; set; }

        public RecentFileList()
        {
            Persister = new RegistryPersister();

            MaxNumberOfFiles = 9;
            MaxPathLength = 50;

            SetMenuItems();
        }
        public void RemoveFile(string filepath)
        {
            Persister.RemoveFile(filepath, MaxNumberOfFiles);
        }
        public void InsertFile(string filepath)
        {
            var files = Items.Where(r => r.File.Filepath.Equals(filepath));
            if (files.Count() > 0)
            {
                RemoveFile(filepath);

                //return;
            };

            Persister.InsertFile(filepath, MaxNumberOfFiles);
            //var vm = RecentFileViewModel.CreateViewModel(new RecentFile(Items.Count + 1, filepath));
            //Items.Add(vm);            
            LoadRecentFiles();
        }

        void SetMenuItems()
        {
            LoadRecentFiles();
        }

        void LoadRecentFiles()
        {
            Clear();
            var files = LoadRecentFilesCore();
            foreach (var item in files)
            {

                Items.Add(RecentFileViewModel.CreateViewModel(item));
            }
        }

        List<RecentFile> LoadRecentFilesCore()
        {
            List<string> list = Persister.RecentFiles(10);

            var files = new List<RecentFile>(list.Count);

            int i = 0;
            foreach (string filepath in list)
            {
                if (File.Exists(filepath))
                    files.Add(new RecentFile(i++, filepath));
                else
                    RemoveFile(filepath);
            }

            return files;
        }

    }

    public class RecentFileViewModel : ViewModelBase
    {
        private RecentFile file;
        private ApplicationViewModel applicationViewModel;
        public RecentFileViewModel(RecentFile file)
        {
            this.file = file;
            applicationViewModel = Resolver.Resolve<ApplicationViewModel>();
            Header = GetHeader(file.Number, file.Filepath, file.DisplayPath);
            CompletePath = file.DisplayPath;
            OpenCommand = new RoutedActionCommand("Open", typeof(RecentFileViewModel))
                              {
                                  Description = "Open Recent Project",
                                  Text = "Open",
                                  Icon = Constants.OPEN_ICON_URL,
                                  OnCanExecute = e => true,
                                  OnExecute = OpenRecent
                              };
        }

        private void OpenRecent(object obj)
        {
            var success = applicationViewModel.ProjectExplorerViewModel.OpenProjectFromFile(file.Filepath);
            if (!success) MessageBox.Show("File not found");
        }

        public RecentFile File
        {
            get { return file; }
        }

        public string Header { get; set; }

        public string CompletePath { get; set; }

        public string GetHeader(int index, string filepath, string displaypath)
        {
            string format = "{2}";
            //string format = "{0}:  {2}";

            string shortPath = StringHelper.ShortenPathname(displaypath, 50);

            return String.Format(format, index, filepath, shortPath);
        }

        public static RecentFileViewModel CreateViewModel(RecentFile file)
        {
            return new RecentFileViewModel(file);
        }

        public RoutedActionCommand OpenCommand { get; private set; }

        public string CommandParameter { get; set; }
        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override Guid Id
        {
            get;
            set;
        }
    }

    public class RecentFile
    {
        public int Number;
        public string Filepath = "";
        public string DisplayPath
        {
            get
            {
                return Path.Combine(
                        Path.GetDirectoryName(Filepath),
                        Path.GetFileNameWithoutExtension(Filepath));
            }
        }

        public RecentFile(int number, string filepath)
        {
            Number = number;
            Filepath = filepath;
        }

        public override string ToString()
        {
            return DisplayPath;
        }
    }

    static class ApplicationAttributes
    {
        static readonly Assembly assembly = null;
        private static readonly Version version = null;

        static readonly AssemblyTitleAttribute title = null;
        static readonly AssemblyCompanyAttribute company = null;
        static readonly AssemblyCopyrightAttribute copyright = null;
        static readonly AssemblyProductAttribute product = null;

        public static string Version { get; private set; }
        public static string Title { get; private set; }
        public static string CompanyName { get; private set; }
        public static string Copyright { get; private set; }
        public static string ProductName { get; private set; }

        static ApplicationAttributes()
        {
            try
            {
                Title = String.Empty;
                CompanyName = String.Empty;
                Copyright = String.Empty;
                ProductName = String.Empty;
                Version = String.Empty;

                assembly = Assembly.GetEntryAssembly();

                if (assembly != null)
                {
                    object[] attributes = assembly.GetCustomAttributes(false);

                    foreach (object attribute in attributes)
                    {
                        Type type = attribute.GetType();

                        if (type == typeof(AssemblyTitleAttribute)) title = (AssemblyTitleAttribute)attribute;
                        if (type == typeof(AssemblyCompanyAttribute)) company = (AssemblyCompanyAttribute)attribute;
                        if (type == typeof(AssemblyCopyrightAttribute)) copyright = (AssemblyCopyrightAttribute)attribute;
                        if (type == typeof(AssemblyProductAttribute)) product = (AssemblyProductAttribute)attribute;
                    }

                    version = assembly.GetName().Version;
                }

                if (title != null) Title = title.Title;
                if (company != null) CompanyName = company.Company;
                if (copyright != null) Copyright = copyright.Copyright;
                if (product != null) ProductName = product.Product;
                if (version != null) Version = version.ToString();
            }
            catch { }
        }
    }


    public class RegistryPersister : IPersist
    {
        public string RegistryKey { get; set; }

        public RegistryPersister()
        {
            RegistryKey =
                    "Software\\" +
                    ApplicationAttributes.CompanyName + "\\" +
                    ApplicationAttributes.ProductName + "\\" +
                    "RecentFileList";
        }

        public RegistryPersister(string key)
        {
            RegistryKey = key;
        }

        static string Key(int i) { return i.ToString("00"); }

        public List<string> RecentFiles(int max)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) k = Registry.CurrentUser.CreateSubKey(RegistryKey);

            var list = new List<string>(max);

            for (int i = 0; i < max; i++)
            {
                string filename = (string)k.GetValue(Key(i));

                if (String.IsNullOrEmpty(filename)) break;

                list.Add(filename);
            }

            return list;
        }


        public void InsertFile(string filepath, int max)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);

            RemoveFile(filepath, max);

            for (int i = max - 2; i >= 0; i--)
            {
                string sThis = Key(i);
                string sNext = Key(i + 1);

                object oThis = k.GetValue(sThis);
                if (oThis == null) continue;

                k.SetValue(sNext, oThis);
            }

            k.SetValue(Key(0), filepath);
        }

        public void RemoveFile(string filepath, int max)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) return;

            for (int i = 0; i < max; i++)
            {
            again:
                string s = (string)k.GetValue(Key(i));
                if (s != null && s.Equals(filepath, StringComparison.CurrentCultureIgnoreCase))
                {
                    RemoveFile(i, max);
                    goto again;
                }
            }
        }

        void RemoveFile(int index, int max)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
            if (k == null) return;

            k.DeleteValue(Key(index), false);

            for (int i = index; i < max - 1; i++)
            {
                string sThis = Key(i);
                string sNext = Key(i + 1);

                object oNext = k.GetValue(sNext);
                if (oNext == null) break;

                k.SetValue(sThis, oNext);
                k.DeleteValue(sNext);
            }
        }
    }

    public class XmlPersister : IPersist
    {
        public string Filepath { get; set; }
        public Stream Stream { get; set; }

        public XmlPersister()
        {
            Filepath =
                    Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            ApplicationAttributes.CompanyName + "\\" +
                            ApplicationAttributes.ProductName + "\\" +
                            "RecentFileList.xml");
        }

        public XmlPersister(string filepath)
        {
            Filepath = filepath;
        }

        public XmlPersister(Stream stream)
        {
            Stream = stream;
        }

        public List<string> RecentFiles(int max)
        {
            return Load(max);
        }


        public void InsertFile(string filepath, int max)
        {
            Update(filepath, true, max);
        }

        public void RemoveFile(string filepath, int max)
        {
            Update(filepath, false, max);
        }

        private void Update(string filepath, bool insert, int max)
        {
            List<string> old = Load(max);

            var list = new List<string>(old.Count + 1);

            if (insert) list.Add(filepath);

            CopyExcluding(old, filepath, list, max);

            Save(list, max);
        }

        private static void CopyExcluding(List<string> source, string exclude, List<string> target, int max)
        {
            foreach (string s in source)
                if (!String.IsNullOrEmpty(s))
                    if (!s.Equals(exclude, StringComparison.OrdinalIgnoreCase))
                        if (target.Count < max)
                            target.Add(s);
        }


        private SmartStream OpenStream(FileMode mode)
        {
            return !String.IsNullOrEmpty(Filepath) ? new SmartStream(Filepath, mode) : new SmartStream(Stream);
        }

        private List<string> Load(int max)
        {
            var list = new List<string>(max);

            using (var ms = new MemoryStream())
            {
                using (SmartStream ss = OpenStream(FileMode.OpenOrCreate))
                {
                    if (ss.Stream.Length == 0) return list;

                    ss.Stream.Position = 0;

                    byte[] buffer = new byte[1 << 20];
                    for (; ; )
                    {
                        int bytes = ss.Stream.Read(buffer, 0, buffer.Length);
                        if (bytes == 0) break;
                        ms.Write(buffer, 0, bytes);
                    }

                    ms.Position = 0;
                }

                XmlTextReader x = null;

                try
                {
                    x = new XmlTextReader(ms);

                    while (x.Read())
                    {
                        switch (x.NodeType)
                        {
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.Whitespace:
                                break;

                            case XmlNodeType.Element:
                                switch (x.Name)
                                {
                                    case "RecentFiles": break;

                                    case "RecentFile":
                                        if (list.Count < max) list.Add(x.GetAttribute(0));
                                        break;

                                    default: Debug.Assert(false); break;
                                }
                                break;

                            case XmlNodeType.EndElement:
                                switch (x.Name)
                                {
                                    case "RecentFiles": return list;
                                    default: Debug.Assert(false); break;
                                }
                                break;

                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                }
                finally
                {
                    if (x != null) x.Close();
                }
            }
            return list;
        }

        private void Save(List<string> list, int max)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlTextWriter x = null;

                try
                {
                    x = new XmlTextWriter(ms, Encoding.UTF8);

                    x.Formatting = Formatting.Indented;

                    x.WriteStartDocument();

                    x.WriteStartElement("RecentFiles");

                    foreach (string filepath in list)
                    {
                        x.WriteStartElement("RecentFile");
                        x.WriteAttributeString("Filepath", filepath);
                        x.WriteEndElement();
                    }

                    x.WriteEndElement();

                    x.WriteEndDocument();

                    x.Flush();

                    using (SmartStream ss = OpenStream(FileMode.Create))
                    {
                        ss.Stream.SetLength(0);

                        ms.Position = 0;

                        byte[] buffer = new byte[1 << 20];
                        for (; ; )
                        {
                            int bytes = ms.Read(buffer, 0, buffer.Length);
                            if (bytes == 0) break;
                            ss.Stream.Write(buffer, 0, bytes);
                        }
                    }
                }
                finally
                {
                    if (x != null) x.Close();
                }
            }
        }
    }
    public class SmartStream : IDisposable
    {
        readonly bool _isStreamOwned = true;
        Stream _stream = null;

        public Stream Stream { get { return _stream; } }

        public static implicit operator Stream(SmartStream me) { return me.Stream; }

        public SmartStream(string filepath, FileMode mode)
        {
            _isStreamOwned = true;

            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            _stream = File.Open(filepath, mode);
        }

        public SmartStream(Stream stream)
        {
            _isStreamOwned = false;
            _stream = stream;
        }

        public void Dispose()
        {
            if (_isStreamOwned && _stream != null) _stream.Dispose();

            _stream = null;
        }
    }

}