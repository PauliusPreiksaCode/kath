import { Dialog, DialogContent, DialogActions, Button, DialogTitle, useTheme } from '@mui/material';
import { Organization } from "../OrganizationList";
import { useDeleteOrganization } from '@/hooks/organization';

interface DeleteOrganizationCardProps {
    open: boolean;
    onClose: () => void;
    organization: Organization;
}

export default function DeleteOrganizationCard ({ open, onClose, organization } : DeleteOrganizationCardProps)  {

    const Theme = useTheme();
    const deleteOrganization = useDeleteOrganization();

    return (
        <Dialog
            open={open}
            onClose={() => onClose()}
        >
            <DialogTitle>Confirm Organization deletion</DialogTitle>
            <DialogContent>
                Are you sure you want to delete organization: {organization?.name}?
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
                            "id": organization.id,
                        };
                        console.log('request', request);
                        
                        await deleteOrganization.mutateAsync(request);
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