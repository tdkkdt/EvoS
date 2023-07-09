import React, {useEffect, useState} from 'react';
import {findPlayers, SearchResults} from "../../lib/Evos";
import {EvosError, processError} from "../../lib/Error";
import {useAuthHeader} from "react-auth-kit";
import {useNavigate, useSearchParams} from "react-router-dom";
import Player from "../atlas/Player";
import {LinearProgress} from "@mui/material";
import ErrorDialog from "../generic/ErrorDialog";
import {FlexBox} from "../generic/BasicComponents";

export default function ProfileSearchPage() {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<EvosError>();
    const [searchResults, setSearchResults] = useState<SearchResults>();

    const [searchParams] = useSearchParams();

    const authHeader = useAuthHeader()();
    const navigate = useNavigate();

    useEffect(() => {
        const query = searchParams.get('query');
        if (!query) return;
        setLoading(true);
        const abort = new AbortController();
        findPlayers(abort, authHeader, query)
            .then((resp) => {
                if (resp.data.players.length === 1) {
                    navigate(`/account/${resp.data.players[0].accountId}`);
                    return;
                }
                setSearchResults(resp.data);
            })
            .catch((error) => processError(error, setError, navigate))
            .then(() => setLoading(false));

        return () => abort.abort();
    }, [searchParams, authHeader, navigate, setSearchResults]);

    return <>
        {error && <ErrorDialog error={error} onDismiss={() => setError(undefined)} />}
        {loading && <LinearProgress />}
        <FlexBox style={{ flexWrap: 'wrap' }}>
            {searchResults && searchResults.players.map(r => <Player info={r} />)}
        </FlexBox>
    </>;
}
