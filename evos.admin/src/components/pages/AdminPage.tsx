import React from 'react';
import PauseQueue from "../controls/PauseQueue";
import Broadcast from "../controls/Broadcast";
import {EvosCard, StackWrapper} from "../generic/BasicComponents";
import IssueRegistrationCode from "../controls/IssueRegistrationCode";

export default function AdminPage() {
    return (
        <StackWrapper>
            <EvosCard variant="outlined">
                <PauseQueue />
            </EvosCard>
            <EvosCard variant="outlined">
                <Broadcast />
            </EvosCard>
            <EvosCard variant="outlined">
                <IssueRegistrationCode />
            </EvosCard>
        </StackWrapper>
    );
}
