import * as alt from 'alt-client';
import { markerStreamer } from "./marker-streamer";

// when an object is streamed in
alt.onServer("entitySync:create", (entityId, entityType, position, currEntityData) => {
    if( currEntityData ) {
        let data = currEntityData;
        if(entityType == 0 ) {
            if (data.player != null) {
                if (data.player != alt.Player.local.id) return;
            }

            markerStreamer.addMarker(
                +entityId, data.markerType, +entityType,
                position, data.rotation,
                data.direction, data.scale, data.color, data.bobUpDown, data.faceCam, data.rotate, data.textureDict, 
                data.textureName, data.drawOnEnter
            );
        }
    }
    else
    {
        if (entityType == 0){
            markerStreamer.restoreMarker( +entityId );
        }
    }
} );

// when an object is streamed out
alt.onServer("entitySync:remove", (entityId, entityType) => {
    if (entityType == 0){
        markerStreamer.removeMarker( +entityId );
    }
} );

// when a streamed in object changes position data
alt.onServer("entitySync:updatePosition", (entityId, entityType, position) => {
    if (entityType == 0){
        markerStreamer.setPosition( +entityId, position );
    }
} );

// when a streamed in object changes data
alt.onServer("entitySync:updateData", (entityId, entityType, newEntityData) => {
    if (entityType == 0){
        if( newEntityData.hasOwnProperty( "rotation" ) )
            markerStreamer.setRotation( +entityId, newEntityData.rotation );

        if( newEntityData.hasOwnProperty( "markerType" ) )
            markerStreamer.setMarkerType( +entityId, newEntityData.markerType );

        if( newEntityData.hasOwnProperty( "drawOnEnter" ) )
            markerStreamer.setDrawOnEnter( +entityId, newEntityData.drawOnEnter );

        if( newEntityData.hasOwnProperty( "textureName" ) )
            markerStreamer.setTextureName( +entityId, newEntityData.textureName );

        if( newEntityData.hasOwnProperty( "textureDict" ) )
            markerStreamer.setTextureDict( +entityId, newEntityData.textureDict );

        if( newEntityData.hasOwnProperty( "rotate" ) )
            markerStreamer.setRotate( +entityId, newEntityData.rotate );

        if( newEntityData.hasOwnProperty( "faceCam" ) )
            markerStreamer.setFaceCamera( +entityId, newEntityData.faceCam );

        if( newEntityData.hasOwnProperty( "bobUpDown" ) )
            markerStreamer.setBobUpDown( +entityId, newEntityData.bobUpDown );

        if( newEntityData.hasOwnProperty( "color" ) )
            markerStreamer.setColor( +entityId, newEntityData.color );

        if( newEntityData.hasOwnProperty( "scale" ) )
            markerStreamer.setScale( +entityId, newEntityData.scale );

        if( newEntityData.hasOwnProperty( "direction" ) )
            markerStreamer.setDirection( +entityId, newEntityData.direction );
    }
} );

// when a streamed in object needs to be removed
alt.onServer("entitySync:clearCache", (entityId, entityType) => {
    if (entityType == 0){
        markerStreamer.clearMarker(+entityId);
    }
});