import { Dialog, DialogContent, DialogActions, Button, DialogTitle, useTheme } from '@mui/material';
import { EntryProps } from '../EntryList';
import { useDeleteFile } from '@/hooks/entry';
import { useContext } from 'react';
import { OrganizationContext } from '@/services/organizationProvider';

interface DeleteFileCardProps {
    open: boolean;
    onClose: () => void;
    entry: EntryProps;
}

export default function DeleteFileCard({ open, onClose, entry } : DeleteFileCardProps) {

    const Theme = useTheme();
    const deleteFile = useDeleteFile();
    const organizationContext = useContext(OrganizationContext);

    return (
        <Dialog
            open={open}
            onClose={() => onClose()}
        >
            <DialogTitle>Confirm file deletion</DialogTitle>
            <DialogContent>
                Are you sure you want to delete this file?
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
                        await deleteFile.mutateAsync({ groupId: organizationContext.groupId, entryId: entry.id });
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