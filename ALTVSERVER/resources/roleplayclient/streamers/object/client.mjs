import * as alt from 'alt-client';

import { objStreamer } from "./object-streamer.mjs";

alt.onServer("entitySync:create", (entityId, entityType, position, currEntityData) => {
    if (entityType == 1) {
        if (currEntityData)
            objStreamer.addObject(+entityId, position, currEntityData);
        else
            objStreamer.restoreObject( +entityId );
    }
});

alt.onServer("entitySync:remove", (entityId, entityType) => {
    if (entityType == 1)
        objStreamer.removeObject(+entityId);
} );

alt.onServer("entitySync:updatePosition", (entityId, entityType, position) => {
    if (entityType == 1)
        objStreamer.setPosition(+entityId, position );
} );

alt.onServer("entitySync:updateData", (entityId, entityType, newEntityData) => {
    if (entityType == 1){
        if( newEntityData.hasOwnProperty( "rotation" ) )
            objStreamer.setRotation( +entityId, newEntityData.rotation );
        else if( newEntityData.hasOwnProperty( "lodDistance" ) )
            objStreamer.setLodDistance( +entityId, newEntityData.lodDistance );
        else if( newEntityData.hasOwnProperty( "textureVariation" ) )
            objStreamer.setTextureVariation( +entityId, newEntityData.textureVariation );
        else if( newEntityData.hasOwnProperty( "visible" ) )
            objStreamer.setVisible( +entityId, newEntityData.visible );
        else if( newEntityData.hasOwnProperty( "onFire" ) )
            objStreamer.setOnFire( +entityId, newEntityData.onFire );
        else if( newEntityData.hasOwnProperty( "freeze" ) )
            objStreamer.setFrozen( +entityId, newEntityData.freeze );
        else if( newEntityData.hasOwnProperty( "lightColor" ) )
            objStreamer.setLightColor( +entityId, newEntityData.lightColor );
        else if (newEntityData.hasOwnProperty("collision"))
            objStreamer.setLightColor(+entityId, newEntityData.collision);
    }
} );

alt.onServer("entitySync:clearCache", (entityId, entityType) => {
    if (entityType == 1)
        objStreamer.clearObject(+entityId);
});