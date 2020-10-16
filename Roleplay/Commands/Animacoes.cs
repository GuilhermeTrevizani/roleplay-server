using AltV.Net.Elements.Entities;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class Animacoes
    {
        [Command("stopanim", Alias = "sa")]
        public void CMD_stopanim(IPlayer player) => Functions.ChecarAnimacoes(player, true);

        [Command("handsup", "/hs (tipo [1-13])", Alias = "hs")]
        public void CMD_hs(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("mp_am_hold_up", "handsup_base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("anim@mp_player_intuppersurrender", "idle_a_fp", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_back_left", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 4:
                    p.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_right", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 5:
                    p.PlayAnimation("missfbi5ig_0", "lyinginpain_loop_steve", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 6:
                    p.PlayAnimation("missfbi5ig_10", "lift_holdup_loop_labped", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 7:
                    p.PlayAnimation("missfbi5ig_17", "walk_in_aim_loop_scientista", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 8:
                    p.PlayAnimation("mp_am_hold_up", "cower_loop", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 9:
                    p.PlayAnimation("mp_arrest_paired", "crook_p1_idle", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 10:
                    p.PlayAnimation("mp_bank_heist_1", "m_cower_02", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 11:
                    p.PlayAnimation("misstrevor1", "threaten_ortega_endloop_ort", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 12:
                    p.PlayAnimation("missminuteman_1ig_2", "handsup_base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                case 13:
                    p.PlayAnimation("anim@mp_player_intincarsurrenderstd@ds@", "idle_a", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 13.");
                    break;
            }
        }

        [Command("crossarms", "/crossarms (tipo [1-2])")]
        public void CMD_crossarms(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_hang_out_street@male_c@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("missheistdockssetup1ig_10@base", "talk_pipe_base_worker2", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("smoke", "/smoke (tipo [1-3])")]
        public void CMD_smoke(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_smoking@male@male_a@enter", "enter", (int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_smoking@male@male_a@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_smoking@male@male_a@exit", "exit", (int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("lean", "/lean (tipo [1-7])")]
        public void CMD_lean(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_leaning@male@wall@back@hands_together@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_leaning@male@wall@back@foot_up@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_leaning@male@wall@back@legs_crossed@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("misscarstealfinale", "packer_idle_base_trevor", (int)(AnimationFlags.Loop));
                    break;
                case 5:
                    p.PlayAnimation("switch@michael@marina", "loop", (int)(AnimationFlags.Loop));
                    break;
                case 6:
                    p.PlayAnimation("switch@michael@pier", "pier_lean_smoke_idle", (int)(AnimationFlags.Loop));
                    break;
                case 7:
                    p.PlayAnimation("switch@michael@sitting_on_car_premiere", "sitting_on_car_premiere_loop_player", (int)(AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 7.");
                    break;
            }
        }

        [Command("police", "/police (tipo [1-6])")]
        public void CMD_police(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@code_human_police_crowd_control@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@code_human_police_crowd_control@idle_a", "idle_a", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@code_human_police_crowd_control@idle_b", "idle_d", (int)(AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("amb@code_human_police_investigate@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 5:
                    p.PlayAnimation("amb@code_human_police_investigate@idle_a", "idle_a", (int)(AnimationFlags.Loop));
                    break;
                case 6:
                    p.PlayAnimation("amb@code_human_police_investigate@idle_b", "idle_f", (int)(AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 6.");
                    break;
            }
        }

        [Command("incar", "/incar (tipo [1-3])")]
        public void CMD_incar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@incar@male@patrol@ds@idle_b", "idle_d", (int)(AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@incar@male@patrol@ds@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@incar@male@patrol@ps@idle_a", "idle_a", (int)(AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("pushups", "/pushups (tipo [1-4])")]
        public void CMD_pushups(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_push_ups@male@enter", "enter", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_push_ups@male@idle_a", "idle_a", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_push_ups@male@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("amb@world_human_push_ups@male@exit", "exit", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 4.");
                    break;
            }
        }

        [Command("situps", "/situps (tipo [1-4])")]
        public void CMD_situps(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_sit_ups@male@enter", "enter", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_sit_ups@male@idle_a", "idle_a", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_sit_ups@male@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("amb@world_human_sit_ups@male@exit", "exit", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 4.");
                    break;
            }
        }

        [Command("blunt", "/blunt (tipo [1-2])")]
        public void CMD_blunt(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_smoking_pot@male@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_smoking_pot@male@idle_a", "idle_a", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("afishing", "/afishing (tipo [1-3])")]
        public void CMD_afishing(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_stand_fishing@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_a", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("acop", "/acop")]
        public void CMD_acop(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("amb@world_human_cop_idles@male@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
        }

        [Command("idle", "/idle (tipo [1-3])")]
        public void CMD_idle(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_drug_dealer_hard@male@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_drug_dealer_hard@male@idle_a", "idle_a", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_drug_dealer_hard@male@idle_b", "idle_d", (int)(AnimationFlags.Loop));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("barra", "/barra (tipo [1-3])")]
        public void CMD_barra(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@prop_human_muscle_chin_ups@male@enter", "enter", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    p.PlayAnimation("amb@prop_human_muscle_chin_ups@male@base", "base", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@prop_human_muscle_chin_ups@male@exit", "exit_flee", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 3.");
                    break;
            }
        }

        [Command("revistarc", "/revistarc")]
        public void CMD_revistarc(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("amb@medic@standing@tendtodead@idle_a", "idle_a", (int)(AnimationFlags.Loop));
        }

        [Command("ajoelhar", "/ajoelhar (tipo [1-3])")]
        public void CMD_ajoelhar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@medic@standing@kneel@enter", "enter", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("amb@medic@standing@tendtodead@base", "base", (int)(AnimationFlags.Loop));
                    break; 
                case 3:
                    p.PlayAnimation("amb@medic@standing@kneel@exit", "exit_flee", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("drink", "/drink (tipo [1-3])")]
        public void CMD_drink(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("amb@world_human_drinking@beer@male@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_drinking@coffee@male@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_drinking@coffee@female@base", "base", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("morto", "/morto (tipo [1-2])")]
        public void CMD_morto(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("missfinale_c1@", "lying_dead_player0", (int)AnimationFlags.Loop);
                    break;
                case 2:
                    p.PlayAnimation("misslamar1dead_body", "dead_idle", (int)AnimationFlags.Loop);
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("gsign", "/gsign (tipo [1-2])")]
        public void CMD_gsign(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("mp_player_int_uppergang_sign_a", "mp_player_int_gang_sign_a", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("mp_player_int_uppergang_sign_b", "mp_player_int_gang_sign_b", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("hurry", "/hurry (tipo [1-2])")]
        public void CMD_hurry(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("missheist_agency3aig_18", "say_hurry_up_a", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("missheist_agency3aig_18", "say_hurry_up_b", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("cair", "/cair (tipo [1-2])")]
        public void CMD_cair(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("mp_bank_heist_1", "prone_l_loop", (int)(AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("mp_bank_heist_1", "prone_r_loop", (int)(AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("wsup", "/wsup (tipo [1-2])")]
        public void CMD_wsup(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("rcmme_amanda1", "pst_arrest_loop_owner", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("missheist_agency2aig_12", "look_at_plan_b", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("render", "/render (tipo [1-2])")]
        public void CMD_render(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("random@arrests@busted", "idle_c", (int)(AnimationFlags.Loop));
                    break; 
                case 2:
                    p.PlayAnimation("random@arrests", "kneeling_arrest_idle", (int)(AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("mirar", "/mirar (tipo [1-2])")]
        public void CMD_mirar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("combat@aim_variations@arrest", "cop_med_arrest_01", (int)(AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("missfbi2", "franklin_sniper_crouch", (int)(AnimationFlags.Loop));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("sentar", "/sentar (tipo [1-8])")]
        public void CMD_sentar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@michael@sitting", "idle", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    p.PlayAnimation("switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_mic", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                case 3:
                    p.PlayAnimation("switch@michael@on_sofa", "base_michael", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                case 4:
                    p.PlayAnimation("safe@franklin@ig_13", "base", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                case 5:
                    p.PlayAnimation("switch@michael@bench", "bench_on_phone_idle", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 6:
                    p.PlayAnimation("switch@michael@parkbench_smoke_ranger", "parkbench_smoke_ranger_loop", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 7:
                    p.PlayAnimation("switch@michael@smoking2", "loop", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                case 8:
                    p.PlayAnimation("switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_jmy", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 8.");
                    break;
            }
        }

        [Command("dormir", "/dormir (tipo [1-2])")]
        public void CMD_dormir(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@franklin@napping", "002333_01_fras_v2_10_napping_idle", (int)(AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    p.PlayAnimation("switch@trevor@dumpster", "002002_01_trvs_14_dumpster_idle", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("pichar", "/pichar (tipo [1-2])")]
        public void CMD_pichar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@franklin@lamar_tagging_wall", "lamar_tagging_wall_loop_lamar", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("switch@franklin@lamar_tagging_wall", "lamar_tagging_exit_loop_lamar", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("sexo", "/sexo (tipo [1-5])")]
        public void CMD_sexo(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@trevor@garbage_food", "loop_trevor", (int)(AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("switch@trevor@head_in_sink", "trev_sink_idle", (int)(AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_stripper", (int)(AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("misscarsteal2pimpsex", "shagloop_hooker", (int)(AnimationFlags.Loop));
                    break;
                case 5:
                    p.PlayAnimation("misscarsteal2pimpsex", "shagloop_pimp", (int)(AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 5.");
                    break;
            }
        }

        [Command("jogado", "/jogado (tipo [1-3])")]
        public void CMD_jogado(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@trevor@slouched_get_up", "trev_slouched_get_up_idle", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("switch@trevor@naked_island", "loop", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 3:
                    p.PlayAnimation("rcm_barry3", "barry_3_sit_loop", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("reparando", "/reparando (tipo [1-2])")]
        public void CMD_reparando(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@trevor@garbage_food", "loop_trevor", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("switch@trevor@puking_into_fountain", "trev_fountain_puke_loop", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("luto", "/luto (tipo [1-2])")]
        public void CMD_luto(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@michael@rejected_entry", "001396_01_mics3_6_rejected_entry_idle_bouncer", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("switch@michael@talks_to_guard", "001393_02_mics3_3_talks_to_guard_idle_guard", (int)(AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("bar")]
        public void CMD_bar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("switch@trevor@bar", "exit_loop_bartender", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
        }

        [Command("necessidades")]
        public void CMD_necessidades(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("switch@trevor@on_toilet", "trev_on_toilet_loop", (int)(AnimationFlags.StopOnLastFrame));
        }

        [Command("meth")]
        public void CMD_meth(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("switch@trevor@trev_smoking_meth", "trev_smoking_meth_loop", (int)(AnimationFlags.StopOnLastFrame));
        }

        [Command("mijar")]
        public void CMD_mijar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("misscarsteal2peeing", "peeing_loop", (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody));
        }

        [Command("dancar", "/dancar (tipo [1-41])")]
        public void CMD_dancar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("anim@amb@nightclub@dancers@club_ambientpeds@med-hi_intensity", "mi-hi_amb_club_10_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 2:
                    p.PlayAnimation("amb@code_human_in_car_mp_actions@dance@bodhi@ds@base", "idle_a_fp", (int)AnimationFlags.Loop);
                    break;
                case 3:
                    p.PlayAnimation("amb@code_human_in_car_mp_actions@dance@bodhi@rds@base", "idle_b", (int)AnimationFlags.Loop);
                    break;
                case 4:
                    p.PlayAnimation("amb@code_human_in_car_mp_actions@dance@std@ds@base", "idle_a", (int)AnimationFlags.Loop);
                    break;
                case 5:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity", "hi_dance_facedj_09_v2_male^6", (int)AnimationFlags.Loop);
                    break;
                case 6:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj@low_intesnsity", "li_dance_facedj_09_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 7:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_hi_intensity", "trans_dance_facedj_hi_to_li_09_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 8:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_low_intensity", "trans_dance_facedj_li_to_hi_07_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 9:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_groups@hi_intensity", "hi_dance_crowd_13_v2_male^6", (int)AnimationFlags.Loop);
                    break;
                case 10:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_groups_transitions@from_hi_intensity", "trans_dance_crowd_hi_to_li__07_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 11:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_single_props@hi_intensity", "hi_dance_prop_13_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 12:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_single_props_transitions@from_med_intensity", "trans_crowd_prop_mi_to_li_11_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 13:
                    p.PlayAnimation("anim@amb@nightclub@mini@dance@dance_solo@male@var_a@", "med_center_up", (int)AnimationFlags.Loop);
                    break;
                case 14:
                    p.PlayAnimation("anim@amb@nightclub@mini@dance@dance_solo@male@var_a@", "med_right_up", (int)AnimationFlags.Loop);
                    break;
                case 15:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_groups@low_intensity", "li_dance_crowd_17_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 16:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_med_intensity", "trans_dance_facedj_mi_to_li_09_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 17:
                    p.PlayAnimation("special_ped@zombie@monologue_4@monologue_4l", "iamtheundead_11", (int)AnimationFlags.Loop);
                    break;
                case 18:
                    p.PlayAnimation("timetable@tracy@ig_5@idle_b", "idle_e", (int)AnimationFlags.Loop);
                    break;
                case 19:
                    p.PlayAnimation("mini@strip_club@idles@dj@idle_04", "idle_04", (int)AnimationFlags.Loop);
                    break;
                case 20:
                    p.PlayAnimation("special_ped@mountain_dancer@monologue_1@monologue_1a", "mtn_dnc_if_you_want_to_get_to_heaven", (int)AnimationFlags.Loop);
                    break;
                case 21:
                    p.PlayAnimation("special_ped@mountain_dancer@monologue_4@monologue_4a", "mnt_dnc_verse", (int)AnimationFlags.Loop);
                    break;
                case 22:
                    p.PlayAnimation("special_ped@mountain_dancer@monologue_3@monologue_3a", "mnt_dnc_buttwag", (int)AnimationFlags.Loop);
                    break;
                case 23:
                    p.PlayAnimation("anim@amb@nightclub@dancers@black_madonna_entourage@", "hi_dance_facedj_09_v2_male^5", (int)AnimationFlags.Loop);
                    break;
                case 24:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_single_props@", "hi_dance_prop_09_v1_male^6", (int)AnimationFlags.Loop);
                    break;
                case 25:
                    p.PlayAnimation("anim@amb@nightclub@dancers@dixon_entourage@", "mi_dance_facedj_15_v1_male^4", (int)AnimationFlags.Loop);
                    break;
                case 26:
                    p.PlayAnimation("anim@amb@nightclub@dancers@podium_dancers@", "hi_dance_facedj_17_v2_male^5", (int)AnimationFlags.Loop);
                    break;
                case 27:
                    p.PlayAnimation("anim@amb@nightclub@dancers@tale_of_us_entourage@", "mi_dance_prop_13_v2_male^4", (int)AnimationFlags.Loop);
                    break;
                case 28:
                    p.PlayAnimation("misschinese2_crystalmazemcs1_cs", "dance_loop_tao", (int)AnimationFlags.Loop);
                    break;
                case 29:
                    p.PlayAnimation("misschinese2_crystalmazemcs1_ig", "dance_loop_tao", (int)AnimationFlags.Loop);
                    break;
                case 30:
                    p.PlayAnimation("anim@mp_player_intcelebrationfemale@uncle_disco", "uncle_disco", (int)AnimationFlags.Loop);
                    break;
                case 31:
                    p.PlayAnimation("anim@mp_player_intcelebrationfemale@raise_the_roof", "raise_the_roof", (int)AnimationFlags.Loop);
                    break;
                case 32:
                    p.PlayAnimation("anim@mp_player_intcelebrationmale@cats_cradle", "cats_cradle", (int)AnimationFlags.Loop);
                    break;
                case 33:
                    p.PlayAnimation("anim@mp_player_intupperbanging_tunes", "idle_a", (int)AnimationFlags.Loop);
                    break;
                case 34:
                    p.PlayAnimation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "high_center", (int)AnimationFlags.Loop);
                    break;
                case 35:
                    p.PlayAnimation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "high_center", (int)AnimationFlags.Loop);
                    break;
                case 36:
                    p.PlayAnimation("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@", "high_center", (int)AnimationFlags.Loop);
                    break;
                case 37:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@", "trans_dance_facedj_hi_to_mi_11_v1_female^6", (int)AnimationFlags.Loop);
                    break;
                case 38:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_hi_intensity", "trans_dance_facedj_hi_to_li_07_v1_female^6", (int)AnimationFlags.Loop);
                    break;
                case 39:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^6", (int)AnimationFlags.Loop);
                    break;
                case 40:
                    p.PlayAnimation("anim@amb@nightclub@dancers@crowddance_groups@hi_intensity", "hi_dance_crowd_09_v1_female^6", (int)AnimationFlags.Loop);
                    break;
                case 41:
                    p.PlayAnimation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_06_base_laz", (int)AnimationFlags.Loop);
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 41.");
                    break;
            }
        }
    }
}