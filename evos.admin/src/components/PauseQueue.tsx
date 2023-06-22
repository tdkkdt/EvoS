import {Button} from "@mui/material";
import {pauseQueue} from "../lib/Evos";
import {FlexBox} from "./BasicComponents";
import {useAuthHeader} from "react-auth-kit";

export default function PauseQueue() {
    const authHeader = useAuthHeader();
    return <>
        <FlexBox style={{ padding: 4 }}>
            <Button
                onClick={() => pauseQueue(authHeader(), true)}
                focusRipple
            >
                Pause queue
            </Button>

            <Button
                onClick={() => pauseQueue(authHeader(), false)}
                focusRipple
            >
                Unpause queue
            </Button>
        </FlexBox>
    </>;
}