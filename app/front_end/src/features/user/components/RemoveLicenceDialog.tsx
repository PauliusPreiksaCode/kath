import { Dialog, DialogContent, DialogActions, Button, DialogTitle, useTheme } from '@mui/material';
import { LicenceLedgerEntry } from "../user";
import { useRemoveLicence } from '@/hooks/user';


interface RemoveLicenceDialogProps {
    open: boolean;
    onClose: () => void;
    licenceLedger: LicenceLedgerEntry | null;
};

export default function RemoveLicenceDialog({ open, onClose, licenceLedger }: RemoveLicenceDialogProps) {

    const removeLicence = useRemoveLicence();
    const Theme = useTheme();

    return (
        <Dialog
                open={open}
                onClose={() => onClose()}
            >
                <DialogTitle>Confirm deactivation of license</DialogTitle>
                <DialogContent>
                    Are you sure you want to deactivate the license?
                </DialogContent>
                <DialogActions>
                    <Button 
                        variant='contained'
                        onClick={() => onClose()} 
                        sx={{
                                backgroundColor: Theme.palette.primary.main,
                                color: Theme.palette.primary.contrastText,
                                fontWeight: 'bold',
                                fontSize: '1rem',
                            }}>
                        Cancel
                    </Button>
                    <Button 
                        variant='contained'
                        onClick={ async () => { 
                            const data = {
                                "licenceLedgerId": licenceLedger?.id,
                            };
                            
                            await removeLicence.mutateAsync(data);
                            onClose(); 
                        }} 
                        sx={{
                            backgroundColor: Theme.palette.primary.main,
                            color: Theme.palette.primary.contrastText,
                            fontWeight: 'bold',
                            fontSize: '1rem',
                        }}>
                        Confirm
                    </Button>
                </DialogActions>
            </Dialog>
    );
}