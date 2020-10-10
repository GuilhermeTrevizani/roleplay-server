using System;

namespace Testador
{
    class Program
    {
        static void Main(string[] args)
        {
            var txt = "const accessories7Female = {";
            for (var i = 0; i <= 14; i++)
                txt += $"{Environment.NewLine}{i}: {{ drawable: {i}, tipoFaccao: 0 }},";
            txt += $"{Environment.NewLine}}};";
        }

        private void MigrarPrecos()
        {
            var str = $@"dagger|2500
hatchet|500
battleaxe|500
knife|3000
machete|1500
stone_hatchet|1000
switchblade|1000
appistol|12000
ceramicpistol|2000
combatpistol|4500
doubleaction|3500
heavypistol|5000
marksmanpistol|1800
pistol|2000
pistol50|4000
snspistol|2300
SNSPistolMkII|2800
vintagepistol|2500
DoubleActionRevolver|36800
HeavyRevolver|3400
HeavyRevolverMkII|4800
PistolMkII|2950
navyrevolver|1300
assaultsmg|13000
combatpdw|10000
machinepistol|8000
microsmg|17000
smg|10500
smgmkii|13000
assaultshotgun|35000
bullpupshotgun|13000
DoubleBarrelShotgun|7000
heavyshotgun|33000
musket|2000
pumpshotgun|7000
pumpshotgunmkii|10000
SawedOffShotgun|7000
SweeperShotgun|33000
advancedrifle|18000
assaultrifle|13000
assaultriflemkii|15000
bullpuprifle|18000
bullpupriflemkii|21000
carbinerifle|13500
carbineriflemkii|17000
compactrifle|35000
specialcarbine|18000
specialcarbinemkii|20000
combatmg|13000
combatmgmkii|15000
GusenbergSweeper|9000
mg|10500
heavysniper|60000
heavysnipermkii|75000
marksmanrifle|50000
marksmanriflemkii|55000
sniperrifle|45000
granade|40000
molotov|5000
pipebombs|85000
ProximityMines|100000
stickybomb|80000";

            var ret = "INSERT INTO Precos VALUES ";
            var arr = str.Split(Environment.NewLine.ToCharArray());
            foreach (var x in arr)
            {
                if (string.IsNullOrWhiteSpace(x))
                    continue;

                var s = x.Split('|');
                ret += $"(7,'{s[0]}',{s[1]}),";
            }
            var a = ret;
        }
    }
}