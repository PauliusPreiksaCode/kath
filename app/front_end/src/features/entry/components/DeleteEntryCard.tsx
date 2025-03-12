import { Dialog, DialogContent, DialogActions, Button, DialogTitle, useTheme } from '@mui/material';
import { EntryProps } from "../EntryList";
import { useDeleteEntry } from '@/hooks/entry';
import { useContext } from 'react';
import { OrganizationContext } from '@/services/organizationProvider';

interface DeleteEntryCardProps {
    open: boolean;
    onClose: () => void;
    entry: EntryProps;
}

export default function DeleteEntryCard({ open, onClose, entry } : DeleteEntryCardProps) {

    const Theme = useTheme();
    const organizationContext = useContext(OrganizationContext);
    const deleteEntry = useDeleteEntry(organizationContext.organizationId);

    return (
        <Dialog
            open={open}
            onClose={() => onClose()}
        >
            <DialogTitle>Confirm Group deletion</DialogTitle>
            <DialogContent>
                Are you sure you want to delete this entry?
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
                        const request = {
                            entryId: entry.id,
                            groupId: organizationContext.groupId,
                        };
                        
                        await deleteEntry.mutateAsync(request);
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