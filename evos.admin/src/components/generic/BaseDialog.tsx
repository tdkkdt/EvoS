import {Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, IconButton} from "@mui/material";
import {DialogProps} from "@mui/material/Dialog/Dialog";
import ContentCopyIcon from '@mui/icons-material/ContentCopy';

interface BaseDialogProps {
    props?: DialogProps;
    title?: string;
    content?: string;
    dismissText?: string;
    onDismiss: () => void;
    copyTitle?: boolean;
}

export default function BaseDialog(
    {
        props,
        title,
        content,
        dismissText,
        onDismiss,
        copyTitle = false
    }: BaseDialogProps) {
    const dialogProps = props ?? {};

    const handleCopyTitle = () => {
        if (title) {
            navigator.clipboard
                .writeText(title)
                .catch(err => console.error('Failed to copy text:', err));
        }
    };

    return (
        <Dialog open={!!title} {...dialogProps}>
            <DialogTitle id="alert-dialog-title">
                {title}
                {copyTitle && title && (
                    <IconButton
                        aria-label="copy title"
                        onClick={handleCopyTitle}
                    >
                        <ContentCopyIcon sx={{ fontSize: '1rem' }} />
                    </IconButton>
                )}
            </DialogTitle>
            {content && <DialogContent>
                <DialogContentText id="alert-dialog-description">{content}</DialogContentText>
            </DialogContent>}
            <DialogActions>
                <Button onClick={onDismiss} autoFocus>{dismissText ?? "Ok"}</Button>
            </DialogActions>
        </Dialog>
    );
}