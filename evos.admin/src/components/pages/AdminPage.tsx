import React from 'react';
import PauseQueue from "../PauseQueue";
import Broadcast from "../Broadcast";
import {EvosCard, StackWrapper} from "../BasicComponents";

export default function AdminPage() {
    return (
        <StackWrapper>
            <EvosCard variant="outlined">
                <PauseQueue />
            </EvosCard>
            <EvosCard variant="outlined">
                <Broadcast />
            </EvosCard>
        </StackWrapper>
    );
}
