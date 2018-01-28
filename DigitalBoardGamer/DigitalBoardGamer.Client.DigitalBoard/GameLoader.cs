using DigitalBoardGamer.Shared.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalBoardGamer.Manager.SettlersManager;
using System.Reflection;

namespace DigitalBoardGamer.Client.DigitalBoard
{
    public class GameLoader
    {
        public GameLoader()
        {
            var throwaway = new SettlersBoardManager();
        }

        public IGameBoardManager LoadGameBoardManager(string dllName)
        {
            if (dllName.ToLower().EndsWith(".dll"))
                dllName = dllName.Substring(0, dllName.Length - ".dll".Length);

            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies();
            var settlerassemblies = assemblies
                .Where(a => a.GetName().Name.ToLower().Contains("settler"));
            var dynamicType = settlerassemblies
                .SingleOrDefault(assembly => assembly.GetName().Name == dllName);

            if (dynamicType != null)
            {
                var allTypes = dynamicType.GetTypes();
                foreach (var currType in allTypes)
                {
                    if (currType.GetInterfaces().Contains(typeof(IGameBoardManager)))
                    {
                        var method = currType.GetConstructor(new Type[] { });//.GetMethod("Bar", BindingFlags.Public);
                        var result = method.Invoke(new object[] { });
                        return (IGameBoardManager)result;
                    }
                }
            }

            return null;
        }
    }
}
