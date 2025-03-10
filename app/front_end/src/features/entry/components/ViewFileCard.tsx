import { alpha, Box, Dialog, DialogContent, DialogActions, Button, DialogTitle, Typography, Grid, IconButton, useTheme } from '@mui/material';
import { EntryProps } from "../EntryList";
import { Close as CloseIcon } from '@mui/icons-material';
import { useDownloadFile } from '@/hooks/entry';
import { useContext, useState } from 'react';
import { OrganizationContext } from '@/services/organizationProvider';
import DeleteFileCard from './DeleteFileCard';


interface ViewFileCardProps {
    open: boolean;
    onClose: () => void;
    entry: EntryProps;
    fullOwner: boolean;
};


export default function ViewFileCard({ open, onClose, entry, fullOwner }: ViewFileCardProps) {

    const Theme = useTheme();
    const dowloadFile = useDownloadFile();
    const organizationContext = useContext(OrganizationContext);
    const [ openDeleteFile, setOpenDeleteFile ] = useState(false);

    const textStyle = {
        mt: '0.5rem',
        fontSize: '1.3rem',
        fontWeight: 'bold',
        color: Theme.palette.primary.main,
        display: 'flex',
        justifyContent: 'center',
    };

    return (
        <>
        <Dialog 
            open={open} 
            onClose={onClose}
            sx={{
                backdropFilter: 'blur(5px)',
                '& .MuiDialogContent-root': {
                    padding: '1.5rem',
                    paddingTop: '0.75rem',
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
                        File
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
                <Grid item xs={12} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}>
                    <Typography sx={textStyle}>Name: {entry.file?.name}</Typography>
                    <Typography sx={textStyle}>Extension: {entry.file?.extension}</Typography>
                    <Typography sx={textStyle}>Upload Date: {entry.file?.uploadDate.split('T')[0]}</Typography>
                </Grid>
            </DialogContent>
            <DialogActions>
                <Grid container spacing={2} sx={{ mb: '1rem', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                    <Grid item xs={6}>
                        <Button
                            variant="contained"
                            sx={{
                                width: '100%',
                                fontSize: '1rem', 
                                fontWeight: 'bold',
                                backgroundColor: Theme.palette.primary.main,
                                color: Theme.palette.primary.contrastText,
                                minHeight: '1rem',
                                maxHeight: '2.5rem',
                            }}
                            onClick={ async () => { 
                                await dowloadFile.mutateAsync({groupId: organizationContext.groupId, entryId: entry.id});
                            }}
                        >
                            Download
                        </Button>
                    </Grid>
                    { fullOwner && (
                        <Grid item xs={6}>
                            <Button
                                variant="contained"
                                sx={{
                                    width: '100%',
                                    fontSize: '1rem', 
                                    fontWeight: 'bold',
                                    backgroundColor: Theme.palette.primary.main,
                                    color: Theme.palette.primary.contrastText,
                                    minHeight: '1rem',
                                    maxHeight: '2.5rem',
                                }}
                                onClick={() => setOpenDeleteFile(true)}
                            >
                                Delete
                            </Button>
                        </Grid>
                    )}
                </Grid>
            </DialogActions>
        </Dialog>
        <DeleteFileCard
            open={openDeleteFile}
            onClose={() => setOpenDeleteFile(false)}
            entry={entry}
        />
        </>
    );
};