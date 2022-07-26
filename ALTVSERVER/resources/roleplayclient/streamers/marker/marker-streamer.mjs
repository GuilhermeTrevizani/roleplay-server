import * as alt from 'alt';
import * as natives from 'natives';

class MarkerStreamer {
    constructor( ) {
        this.markers = {};
    }

    async addMarker( entityId, markerType, entityType, pos, rot, dir, scale, color, bobUpDown, faceCam, rotate, textureDict, textureName, drawOnEnter ) {
        this.removeMarker( +entityId );
        this.clearMarker( +entityId );

        let marker = {
            onDisplay: true, markerType: markerType,
            entityId: +entityId, entityType: +entityType, position: pos
        };

        this.markers[entityId] = marker;

        this.setRotation( +entityId, rot );
        this.setDirection( +entityId, dir );
        this.setScale( +entityId, scale );
        this.setColor( +entityId, color );
        this.setBobUpDown( +entityId, bobUpDown );
        this.setFaceCamera( +entityId, faceCam );
        this.setRotate( +entityId, rotate );
        this.setTextureDict( +entityId, textureDict );
        this.setTextureName( +entityId, textureName );
        this.setDrawOnEnter( +entityId, drawOnEnter );
    }

    getMarker( entityId ) {
      if(this.markers.hasOwnProperty(entityId)){
        return this.markers[entityId];
      }else{
        return null;
      }
    }

    restoreMarker( entityId ) {
      if(this.markers.hasOwnProperty(entityId)){
        this.markers[entityId].onDisplay = true;
      }
    }


    removeMarker( entityId ) {
      if(this.markers.hasOwnProperty(entityId)){
        this.markers[entityId].onDisplay = false;
      }
    }

    clearMarker( entityId ) {
      if(this.markers.hasOwnProperty(entityId)){
        delete this.markers[entityId];
      }
    }

    clearAllMarker( ) {
      this.markers = {};
    }

    setMarkerType( entityId, type = 0 ) {
      if(this.markers.hasOwnProperty(entityId)){
        this.markers[entityId].markerType = type;
      }
    }


    setTextureName( entityId, textureName = undefined ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].textureName = textureName;
        }
    }

    setTextureDict( entityId, textureDict = undefined ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].textureDict = textureDict;
        }
    }

    setDrawOnEnter( entityId, drawOnEnter = false ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].drawOnEnter = drawOnEnter;
        }
    }

    setRotate( entityId, rotate = false ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].rotate = rotate;
        }
    }

    setFaceCamera( entityId, faceCam = false) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].faceCam = faceCam;
        }
    }

    setBobUpDown( entityId, bobUpDown = false) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].bobUpDown = bobUpDown;
        }
    }

    setColor( entityId, color = { r: 255, g: 255, b: 255, a: 255 } ) {
      if(this.markers.hasOwnProperty(entityId)){
        this.markers[entityId].color = color;
      }
    }

    setScale( entityId, scale = { x: 0, y: 0, z: 0 } ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].scale = scale;
        }
    }

    setDirection( entityId, dir = { x: 0, y: 0, z: 0 } ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].direction = dir;
        }
    }

    setRotation( entityId, rot = { x: 0, y: 0, z: 0 } ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].rotation = rot;
        }
    }

    setPosition( entityId, pos = { x: 0, y: 0, z: 0 }  ) {
        if(this.markers.hasOwnProperty(entityId)){
          this.markers[entityId].position = pos;
        }
    }
}

export const markerStreamer = new MarkerStreamer();

alt.on( "resourceStop", ( ) => {
    markerStreamer.clearAllMarker();
});

alt.everyTick(() => {
  if (alt.Player.local.getMeta('f7')) return;

  for(var key in  markerStreamer.markers) {
    let marker = markerStreamer.markers[key];
    if(marker.onDisplay) {
      natives.drawMarker(
        marker.markerType, marker.position.x, marker.position.y, marker.position.z,
        marker.direction.x, marker.direction.y, marker.direction.z,
        marker.rotation.x, marker.rotation.y, marker.rotation.z,
        marker.scale.x, marker.scale.y, marker.scale.z,
        marker.color.r, marker.color.g, marker.color.b, marker.color.a,
        !!marker.bobUpDown, !!marker.faceCam, 2, !!marker.rotate, marker.textureDict, marker.textureName, !!marker.drawOnEnter
      );
    }
  }
} );