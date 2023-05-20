using DuckGame;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace ArmoryPlus.src.Core
{
    [UsedImplicitly]
    public class ArmoryPlus:Mod
    {
        [UsedImplicitly]
        internal static ArmoryPlus LastInstance;

        public ArmoryPlus()
        {
            Debug.Log("Armory+ loading");
            /*AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;*/
            LastInstance = this;
        }

        public override Priority priority => Priority.Normal;

        //Происходит перед запуском мода
        /*protected override void OnPreInitialize()
        {
            base.OnPreInitialize();
        }*/

        /*private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyname = new AssemblyName(args.Name).Name;
            var assemblyFileName = Path.Combine(configuration.directory, assemblyname + ".dll");
            var assembly = Assembly.LoadFrom(assemblyFileName);
            return assembly;
        }*/

        //Происходит после запуска мода
        /*
        protected override void OnPostInitialize()
        { 
            base.OnPostInitialize();
        }
        */

        // ReSharper disable once InconsistentNaming
        private static IEnumerable<byte> GetMD5Hash(byte[] sourceBytes)
        {
            return new MD5CryptoServiceProvider().ComputeHash(sourceBytes);
        }


    }
}
