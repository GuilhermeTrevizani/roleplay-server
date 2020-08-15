using AltV.Net.Elements.Entities;
using Roleplay.Models;

namespace Roleplay.Commands
{
    public class Anims
    {
        [Command("stopanim", "/stopanim", Alias = "sa")]
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
                    p.PlayAnimation("mp_am_hold_up", "handsup_base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("anim@mp_player_intuppersurrender", "idle_a_fp", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_back_left", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 4:
                    p.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_right", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 5:
                    p.PlayAnimation("missfbi5ig_0", "lyinginpain_loop_steve", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 6:
                    p.PlayAnimation("missfbi5ig_10", "lift_holdup_loop_labped", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 7:
                    p.PlayAnimation("missfbi5ig_17", "walk_in_aim_loop_scientista", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 8:
                    p.PlayAnimation("mp_am_hold_up", "cower_loop", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 9:
                    p.PlayAnimation("mp_arrest_paired", "crook_p1_idle", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 10:
                    p.PlayAnimation("mp_bank_heist_1", "m_cower_02", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 11:
                    p.PlayAnimation("misstrevor1", "threaten_ortega_endloop_ort", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 12:
                    p.PlayAnimation("missminuteman_1ig_2", "handsup_base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 13:
                    p.PlayAnimation("anim@mp_player_intincarsurrenderstd@ds@", "idle_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
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
                    p.PlayAnimation("amb@world_human_hang_out_street@male_c@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("missheistdockssetup1ig_10@base", "talk_pipe_base_worker2", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("amb@world_human_smoking@male@male_a@enter", "enter", (int)(Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_smoking@male@male_a@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_smoking@male@male_a@exit", "exit", (int)(Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("amb@world_human_leaning@male@wall@back@hands_together@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_leaning@male@wall@back@foot_up@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_leaning@male@wall@back@legs_crossed@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("misscarstealfinale", "packer_idle_base_trevor", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 5:
                    p.PlayAnimation("switch@michael@marina", "loop", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 6:
                    p.PlayAnimation("switch@michael@pier", "pier_lean_smoke_idle", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 7:
                    p.PlayAnimation("switch@michael@sitting_on_car_premiere", "sitting_on_car_premiere_loop_player", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("amb@code_human_police_crowd_control@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@code_human_police_crowd_control@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@code_human_police_crowd_control@idle_b", "idle_d", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("amb@code_human_police_investigate@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 5:
                    p.PlayAnimation("amb@code_human_police_investigate@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 6:
                    p.PlayAnimation("amb@code_human_police_investigate@idle_b", "idle_f", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("amb@incar@male@patrol@ds@idle_b", "idle_d", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@incar@male@patrol@ds@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@incar@male@patrol@ps@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("amb@world_human_push_ups@male@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_push_ups@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_push_ups@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("amb@world_human_push_ups@male@exit", "exit", (int)(Constants.AnimationFlags.StopOnLastFrame));
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
                    p.PlayAnimation("amb@world_human_sit_ups@male@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_sit_ups@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_sit_ups@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("amb@world_human_sit_ups@male@exit", "exit", (int)(Constants.AnimationFlags.StopOnLastFrame));
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
                    p.PlayAnimation("amb@world_human_smoking_pot@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_smoking_pot@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("amb@world_human_stand_fishing@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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

            p.PlayAnimation("amb@world_human_cop_idles@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("amb@world_human_drug_dealer_hard@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_drug_dealer_hard@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_drug_dealer_hard@male@idle_b", "idle_d", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("amb@prop_human_muscle_chin_ups@male@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    p.PlayAnimation("amb@prop_human_muscle_chin_ups@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("amb@prop_human_muscle_chin_ups@male@exit", "exit_flee", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 3.");
                    break;
            }
        }

        [Command("kneel", "/kneel")]
        public void CMD_kneel(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("amb@medic@standing@tendtodead@base", "base", (int)(Constants.AnimationFlags.Loop));
        }

        [Command("revistarc", "/revistarc")]
        public void CMD_revistarc(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("amb@medic@standing@tendtodead@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
        }

        [Command("ajoelhar", "/ajoelhar (tipo [1-4])")]
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
                    p.PlayAnimation("amb@medic@standing@kneel@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("amb@medic@standing@kneel@base", "base", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 3:
                    p.PlayAnimation("amb@medic@standing@kneel@idle_a", "idle_a", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 4:
                    p.PlayAnimation("amb@medic@standing@kneel@exit", "exit_flee", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 4.");
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
                    p.PlayAnimation("amb@world_human_drinking@beer@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("amb@world_human_drinking@coffee@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    p.PlayAnimation("amb@world_human_drinking@coffee@female@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("missfinale_c1@", "lying_dead_player0", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("misslamar1dead_body", "dead_idle", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("mp_player_int_uppergang_sign_a", "mp_player_int_gang_sign_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("mp_player_int_uppergang_sign_b", "mp_player_int_gang_sign_b", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("missheist_agency3aig_18", "say_hurry_up_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("missheist_agency3aig_18", "say_hurry_up_b", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("mp_bank_heist_1", "prone_l_loop", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("mp_bank_heist_1", "prone_r_loop", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("rcmme_amanda1", "pst_arrest_loop_owner", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("missheist_agency2aig_12", "look_at_plan_b", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("random@arrests@busted", "idle_c", (int)(Constants.AnimationFlags.Loop));
                    break; 
                case 2:
                    p.PlayAnimation("random@arrests", "kneeling_arrest_idle", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("combat@aim_variations@arrest", "cop_med_arrest_01", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("missfbi2", "franklin_sniper_crouch", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("switch@michael@sitting", "idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    p.PlayAnimation("switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_mic", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 3:
                    p.PlayAnimation("switch@michael@on_sofa", "base_michael", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 4:
                    p.PlayAnimation("safe@franklin@ig_13", "base", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 5:
                    p.PlayAnimation("switch@michael@bench", "bench_on_phone_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 6:
                    p.PlayAnimation("switch@michael@parkbench_smoke_ranger", "parkbench_smoke_ranger_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 7:
                    p.PlayAnimation("switch@michael@smoking2", "loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 8:
                    p.PlayAnimation("switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_jmy", (int)(Constants.AnimationFlags.StopOnLastFrame));
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
                    p.PlayAnimation("switch@franklin@napping", "002333_01_fras_v2_10_napping_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    p.PlayAnimation("switch@trevor@dumpster", "002002_01_trvs_14_dumpster_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("pixar", "/pixar (tipo [1-2])")]
        public void CMD_pixar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    p.PlayAnimation("switch@franklin@lamar_tagging_wall", "lamar_tagging_wall_loop_lamar", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    p.PlayAnimation("switch@franklin@lamar_tagging_wall", "lamar_tagging_exit_loop_lamar", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
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
                    p.PlayAnimation("switch@trevor@garbage_food", "loop_trevor", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    p.PlayAnimation("switch@trevor@head_in_sink", "trev_sink_idle", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    p.PlayAnimation("switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_stripper", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    p.PlayAnimation("misscarsteal2pimpsex", "shagloop_hooker", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 5:
                    p.PlayAnimation("misscarsteal2pimpsex", "shagloop_pimp", (int)(Constants.AnimationFlags.Loop));
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
                    p.PlayAnimation("switch@trevor@slouched_get_up", "trev_slouched_get_up_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("switch@trevor@naked_island", "loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 3:
                    p.PlayAnimation("rcm_barry3", "barry_3_sit_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
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
                    p.PlayAnimation("switch@trevor@garbage_food", "loop_trevor", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("switch@trevor@puking_into_fountain", "trev_fountain_puke_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
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
                    p.PlayAnimation("switch@michael@rejected_entry", "001396_01_mics3_6_rejected_entry_idle_bouncer", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    p.PlayAnimation("switch@michael@talks_to_guard", "001393_02_mics3_3_talks_to_guard_idle_guard", (int)(Constants.AnimationFlags.StopOnLastFrame));
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

            p.PlayAnimation("switch@trevor@bar", "exit_loop_bartender", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
        }

        [Command("necessidades")]
        public void CMD_necessidades(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("switch@trevor@on_toilet", "trev_on_toilet_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
        }

        [Command("meth")]
        public void CMD_meth(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("switch@trevor@trev_smoking_meth", "trev_smoking_meth_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
        }

        [Command("mijar")]
        public void CMD_mijar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (!Functions.ChecarAnimacoes(player))
                return;

            p.PlayAnimation("misscarsteal2peeing", "peeing_loop", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
        }
    }
}