import {PlayerData} from "../lib/Evos";
import {Button} from "@mui/material";

interface Props {
    info?: PlayerData;
}

function Player({info}: Props) {
    return <>
        <Button variant="text">{info?.handle ?? "UNKNOWN"}</Button>
    </>;
}

export default Player;