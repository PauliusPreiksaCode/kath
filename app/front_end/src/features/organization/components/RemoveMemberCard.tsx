import { alpha, Autocomplete, Box, CircularProgress, Dialog, DialogContent, DialogActions, Button, DialogTitle, TextField, Grid, IconButton, useTheme } from '@mui/material';
import { useEffect, useState } from "react";
import { Organization } from "../OrganizationList";
import { useGetOrganizationUsers, useRemoveUserFromOrganization } from "@/hooks/organization";
import { Close as CloseIcon } from '@mui/icons-material';

interface RemoveMemberCardProps {
    open: boolean;
    onClose: () => void;
    organization: Organization | null;
};

export default function RemoveMemberCard({ open, onClose, organization } : RemoveMemberCardProps) {

    const [users, setUsers] = useState<any>([]);
    const [selectedUser, setSelectedUser] = useState<any>(null);
    const [confirmOpen, setConfirmOpen] = useState(false);
    const Theme = useTheme();

    const getOrganizationUsers = useGetOrganizationUsers(organization?.id);
    const removeUser = useRemoveUserFromOrganization();

    useEffect(() => {
        setUsers(getOrganizationUsers.data || [])
    }, [getOrganizationUsers.data]);

    if(getOrganizationUsers.isLoading || getOrganizationUsers.isFetching) {
        return <CircularProgress/>;
    }

    const handleUserSelect = (_: any, newValue: any) => {
        setSelectedUser(newValue);
        if (newValue) {
            setConfirmOpen(true);
        }
    };

    return (
            <Dialog 
                open={open} 
                onClose={onClose}
                sx={{
                    backdropFilter: 'blur(5px)',
                    '& .MuiDialogContent-root': {
                        padding: '1.5rem',
                    },
                    '& .MuiDialogActions-root': {
                        padding: '1.5rem',
                    },
                    '& .MuiDialog-paper': {
                        borderRadius: '1.5rem',
                        minWidth: '25%',
                        backgroundColor: Theme.palette.background.paper,
                        backgroundImage: 'none',
                    }
                }}
            >
                <Grid container spacing={2} justifyContent='center' alignItems='center'>
                    <Grid item xs={8}>
                        <DialogTitle
                            sx={{
                            color: Theme.palette.primary.main,
                            pl: '1.5rem',
                            pt: '1.5rem',
                            fontWeight: '700',
                            fontSize: '1.2rem',
                            }}
                        >
                            Remove Member
                        </DialogTitle>
                    </Grid>
                    <Grid item xs={4}>
                        <Box display='flex' justifyContent='flex-end'>
                            <IconButton
                                aria-label='close'
                                onClick={onClose}
                                sx={{
                                    color: Theme.palette.primary.main,
                                    mt: '0.5rem',
                                    mr: '1.5rem',
                                }}
                            >
                                <CloseIcon />
                            </IconButton>
                        </Box>
                    </Grid>
                </Grid>
                <DialogContent sx={{ borderTop: `1px solid ${alpha(Theme.palette.text.secondary, 0.3)}` }}>
                <Autocomplete
                        size="small"
                        sx={(theme) => ({
                            '& fieldset': {
                                borderColor: theme.palette.text.primary,
                                borderRadius: '1rem',
                            },
                        })}
                        options={users}
                        getOptionLabel={(option) => option.email}
                        renderInput={(params) => 
                            <Box sx={{ px: '0.5rem' }}>
                                <TextField
                                    {...params}
                                    label="User"
                                />
                            </Box>
                        }
                        onChange={handleUserSelect}
                    />
                </DialogContent>
                <Dialog
                    open={confirmOpen}
                    onClose={() => setConfirmOpen(false)}
                >
                    <DialogTitle>Confirm removing user</DialogTitle>
                    <DialogContent>
                        Are you sure you want to remove user {selectedUser?.email}?
                    </DialogContent>
                    <DialogActions>
                        <Button 
                            variant='contained'
                            onClick={() => setConfirmOpen(false)} 
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
                                    "userId": selectedUser.id,
                                    "organizationId": organization?.id,
                                };
                                
                                await removeUser.mutateAsync(data);
                                setConfirmOpen(false); 
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
            </Dialog>
        );
}