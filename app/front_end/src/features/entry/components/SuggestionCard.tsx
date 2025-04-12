import { alpha, Box, Dialog, DialogContent, DialogActions, DialogTitle, Typography, Grid, IconButton, useTheme, CircularProgress } from '@mui/material';
import { Close as CloseIcon, AutoAwesome as AIIcon } from '@mui/icons-material';
import { useSuggestLinkEntries } from '@/hooks/entry';
import { useContext, useEffect, useState } from 'react';
import { OrganizationContext } from '@/services/organizationProvider';

interface SuggestionCardProps {
    open: boolean;
    onClose: () => void;
    text: string;
    entryId?: string;
};

export default function SuggestionCard({ open, onClose, text, entryId }: SuggestionCardProps) {

    const Theme = useTheme();
    const organizationContext = useContext(OrganizationContext);
    const suggestEntries = useSuggestLinkEntries();
    const [suggestions, setSuggestions] = useState([]);

    var results = async () => await suggestEntries.mutateAsync({ "text": text, "groupId": organizationContext.groupId, "entryId": entryId });

    useEffect(() => {
        if (open) {
            results().then((data) => {
                setSuggestions(data || []);
            });
        }
    }, [open]);

    const textStyle = {
        mt: '0.5rem',
        fontSize: '1.1rem',
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
                    minWidth: '50%',
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
                        display: 'flex',
                        alignItems: 'center',
                        }}
                    >
                        <span>Suggest related links with AI</span><AIIcon sx={{ ml: 1 }} />
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
                    {suggestions.length > 0 ? 
                        suggestions?.map((suggestion: any) => <Typography sx={textStyle}>{suggestion.title} - {suggestion.reason}</Typography>) :
                        <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", height: "10vh", width: "100%" }}>
                          <CircularProgress />
                        </Box>
                    }
                </Grid>
            </DialogContent>
            <DialogActions>
                <Grid container spacing={2} sx={{ mb: '1rem', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                    <Grid item xs={6}>
                    </Grid>
                </Grid>
            </DialogActions>
        </Dialog>
        </>
    );
};