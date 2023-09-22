import * as alt from 'alt-client';
import * as native from 'natives';

const maxCountLoadTry = 255;
const player = alt.Player.local;

alt.onServer('animation:Clear', clearAnimation);
function clearAnimation() {
  player.deleteMeta('animation_dic');
  player.deleteMeta('animation_name');
  player.deleteMeta('animation_duration');
  player.deleteMeta('animation_flag');
  player.deleteMeta('animation_freeze');
}

function checkAnimation(dict, name) {
  return new Promise(resolve => {
    let count = 0;
    let inter = alt.setInterval(() => {
      if (count > maxCountLoadTry) {
        alt.clearInterval(inter);
        return;
      }

      if (native.isEntityPlayingAnim(player, dict, name, 3)) {
        resolve(true);
        alt.clearInterval(inter);
        return;
      }

      count += 1;
    }, 5);
  });
}

function setAnimation(dict, name, duration, flag, freeze) {
  player.setMeta('animation_duration', duration);
  player.setMeta('animation_flag', flag);
  player.setMeta('animation_freeze', freeze);
  player.setMeta('animation_dic', dict);
  player.setMeta('animation_name', name);
}

alt.onServer('animation:Play', playAnimation);
export async function playAnimation(dict, name, flag, duration, freeze) {
  clearAnimation();

  alt.Utils.requestAnimDict(dict).then(() => {
    native.taskPlayAnim(
      player,
      dict,
      name,
      1,
      -1,
      duration,
      flag,
      1.0,
      false,
      false,
      false
    );

    checkAnimation(dict, name).then(() => {
      setAnimation(dict, name, duration, flag, freeze);
    });
  });
}