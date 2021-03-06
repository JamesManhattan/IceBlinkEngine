using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using IceBlinkCore;
using IceBlink;
using System.IO;

namespace IceBlink
{
    public class IceBlinkScript
    {
        public void Script(ScriptFunctions sf, string p1, string p2, string p3, string p4)
        {
            // C# code goes here
            //This will summon a Prop by Tag, it requires a custom script in Game.cs called DisposePCOnlyCombatSpritesTextures(); which is just the foreach PC loop of DisposeCombatSpritesTextures();
            //a copy of DisposePCOnlyCombatSpritesTextures is included in comments at very bottom of this file.
            //it is required to be a "Point" target spell.
            if (sf.CombatSource is PC)
            {
                PC source = (PC)sf.CombatSource;
                //Creature target = (Creature)sf.CombatTarget;
                Point target = (Point)sf.CombatTarget;
                Combat c = sf.frm.currentCombat;

                Creature summon = null;

                summon = sf.gm.module.ModuleCreaturesList.getCreatureByTag("skele");
                //c.logText(summon.Name + " has been picked" + Environment.NewLine, Color.YellowGreen);

                // * add ResRefs to encounter and add creatures
                int order = c.currentMoveOrderIndex;
                Creature crt = new Creature();
                CreatureRefs crt_ref = null;
                int count = 1;

                for (int i = 0; i < count; i++)
                {
                    // * ResRef
                    crt_ref = new CreatureRefs();
                    crt_ref.CreatureResRef = summon.ResRef;
                    crt_ref.CreatureName = summon.Name + " Ally";
                    crt_ref.CreatureTag = summon.Tag + "Ally" + i;
                    ////crt_ref.CreatureStartLocation = new Point(frm.currentEncounter.EncounterPcStartLocations[0].X + gm.Random(6)-3,
                    ////                             frm.currentEncounter.EncounterPcStartLocations[0].Y + 5 + gm.Random(3)-2);
                    //crt_ref.CreatureStartLocation = new Point(0, i + 1);

                    sf.gm.currentEncounter.EncounterCreatureRefsList.Add(crt_ref);

                    // * creature
                    crt = summon.DeepCopy();
                    crt.Tag = summon.Tag + "Ally" + i;
                    //crt.CombatLocation = new Point(0, i + 1);               
                    //check if there is a creature already in the square chosen, then find nearest empty spot at random.
                    //int j = 0;
                    //int k = 0;
                    //do
                    //{
                    //    j = sf.gm.Random(-1, 1);
                    //    k = sf.gm.Random(-1, 1);
                    //    target.X = target.X + j;
                    //    target.Y = target.Y + k;
                    //} while (c.checkPointCollision(target)) ;    //this was a custom function to see if square is already occupied in Combat.cs                                    

                    crt.CombatLocation = target; 

                    crt.Tag = "Summoned " + summon.Name + " " + i; // * need to check for further summonings!
                    crt.OnStartCombatTurn.FilenameOrTag = "crtPCAllyOnStartCombatTurn.cs"; //overwrite the AI to make it friendly to the players.
                   
                    //below line is necessary
                    if (File.Exists(sf.gm.mainDirectory + "\\modules\\" + sf.gm.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + crt.SpriteFilename)) 
                    {
                        crt.LoadCharacterSprite(sf.gm.mainDirectory + "\\modules\\" + sf.gm.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\module",  crt.SpriteFilename);
                    }
                    else if (File.Exists(sf.gm.mainDirectory + "\\data\\graphics\\sprites\\tokens\\" + crt.SpriteFilename)) 
                    {
                        crt.LoadCharacterSprite(sf.gm.mainDirectory + "\\data\\graphics\\sprites\\tokens", crt.SpriteFilename);
                    }
                    else
                    {
                        c.logText("Could not find sprite for " + crt.SpriteFilename + Environment.NewLine, Color.YellowGreen);
                    }                   

                    sf.gm.currentEncounter.EncounterCreatureList.creatures.Add(crt);
                    //put it into the initiative tracker at the very end.
                    MoveOrder mo = new MoveOrder();
                    mo.index = sf.gm.currentEncounter.EncounterCreatureList.creatures.Count - 1;
                    mo.type = "creature";
                    mo.tag = crt.Tag;
                    mo.rank = 0; // sf.gm.Random(20);// gm.Random(100) + (dexMod * 10) + Stats.CalcInitiativeBonuses(chr); 
                    c.com_moveOrderList.Add(mo);

                    c.logText(crt.Name + " has been summoned" + Environment.NewLine, Color.YellowGreen);
                }

                sf.gm.DisposePCOnlyCombatSpritesTextures(); //get rid of the PC textures because they are "ADDED" back in the grand Initialize function after this.
                Thread.Sleep(200); //this allows all the floaty text to float away for its alloted time, which would cause rendering issues if not let to finish.
                sf.gm.InitializeCombatRenderPanel(sf.gm.combatRenderPanel);  //WORKS!
                
            }
            else if (sf.CombatSource is Creature)
            {
                Creature source = (Creature)sf.CombatSource;
                ////PC target = (PC)sf.CombatTarget;
                Point target = (Point)sf.CombatTarget;
                Combat c = sf.frm.currentCombat;
                
            }
            else // don't know who cast this spell
            {
                IBMessageBox.Show(sf.gm, "Invalid script owner, not a Creature or PC");
                return;
            }
        }
        
    }
    //This function needs to be put into Game.cs and then a new IceBlinkCore.dll compiled from it.
    //public void DisposePCOnlyCombatSpritesTextures()
    //    {
    //        foreach (SharpDX.Direct3D9.Sprite spr in pcCombatSprites)
    //        {
    //            spr.Dispose();
    //        }    //        
    //        //clear the floaty text pools because they cause issues.
    //        shadowCombatTextPool.Clear();
    //    }
}
