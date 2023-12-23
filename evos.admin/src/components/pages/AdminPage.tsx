import React from 'react';
import PauseQueue from "../controls/PauseQueue";
import Broadcast from "../controls/Broadcast";
import {EvosCard, StackWrapper} from "../generic/BasicComponents";
import Shutdown from "../controls/Shutdown";

export default function AdminPage() {
    return (
        <StackWrapper>
            <EvosCard variant="outlined">
                <PauseQueue />
            </EvosCard>
            <EvosCard variant="outlined">
                <Shutdown />
            </EvosCard>
            <EvosCard variant="outlined">
                <Broadcast />
            </EvosCard>
        </StackWrapper>
    );
}
