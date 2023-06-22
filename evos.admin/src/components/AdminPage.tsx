import React from 'react';
import PauseQueue from "./PauseQueue";
import Broadcast from "./Broadcast";

export default function AdminPage() {
    return (
        <div className="App">
            <header className="App-header">
                <PauseQueue />
                <Broadcast />
            </header>
        </div>
    );
}
