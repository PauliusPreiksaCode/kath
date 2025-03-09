import { Dialog, DialogContent, DialogActions, Button, DialogTitle, useTheme } from '@mui/material';
import { GroupProps } from '../GroupList';
import { useDeleteGroup } from '@/hooks/group';

interface DeleteGroupCardProps {
    open: boolean;
    onClose: () => void;
    group: GroupProps;
}

export default function DeleteGroupCard({ open, onClose, group } : DeleteGroupCardProps) {

    const Theme = useTheme();
    const deleteGroup = useDeleteGroup();

    return (
        <Dialog
            open={open}
            onClose={() => onClose()}
        >
            <DialogTitle>Confirm Group deletion</DialogTitle>
            <DialogContent>
                Are you sure you want to delete group: {group?.name}?
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
                            organizationId: group.organizationId,
                            groupId: group.id,
                        };
                        
                        await deleteGroup.mutateAsync(request);
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
};
