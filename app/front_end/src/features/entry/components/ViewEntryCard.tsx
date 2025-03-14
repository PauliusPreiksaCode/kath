import { Typography, Card, CardContent, useTheme, Dialog, Box, DialogTitle, Grid, IconButton, alpha, CircularProgress, Chip } from "@mui/material";
import { useContext, useEffect, useState, forwardRef } from "react";
import { useGetEntry } from "@/hooks/entry";
import { OrganizationContext } from "@/services/organizationProvider";
import { Close as CloseIcon } from '@mui/icons-material';
import FileOpenIcon from '@mui/icons-material/FileOpen';
import ViewFileCard from "./ViewFileCard";
import ReactMarkdown from 'react-markdown';
import rehypeHighlight from 'rehype-highlight';
import 'highlight.js/styles/github.css';
import { CodeProps } from "./CreateEntryCard";

interface ViewEntryCardProps {
    open: boolean;
    onClose: () => void;
    entryId: string | undefined;
}

interface ViewEntryProps {
    id: string;
    name: string;
    text: string;
    fullName: string;
    creationDate: string;
    modifyDate: string;
    licencedUserId: string;
    fileId: string;
    file: {
        id: string;
        name: string;
        extension: number;
        uploadDate: string;
    }
    linkedEntries: LinkedEntryProps[];
}

interface LinkedEntryProps{
    id: string;
    name: string;
}

export default function ViewEntryCard({open, onClose, entryId} : ViewEntryCardProps) {

    const theme = useTheme();
    const organizationContext = useContext(OrganizationContext);
    const getEntry = useGetEntry(organizationContext.organizationId, entryId ?? '');
    const [entry , setEntry] = useState<ViewEntryProps>();
    const [openFileManagment, setOpenFileManagment] = useState<boolean>(false);
    const [selectedEntryId, setSelectedEntry] = useState<string>('');
    const [openEntry, setOpenEntry] = useState<boolean>(false);


    useEffect(() => {
        setEntry(getEntry.data || null);
    }, [getEntry.data]);

    if((getEntry.isLoading && !getEntry.isFetching)) {
            return <CircularProgress/>;
    }

    const hasFile = entry?.fileId !== null;

    const textStyle = {
        fontSize: '1.4rem',
        fontWeight: 'bold',
        color: theme.palette.primary.main,
        display: 'flex',
        justifyContent: 'center',
    };

    return (
        <Dialog 
            open={open} 
            onClose={onClose}
            fullWidth={true}
            maxWidth="md"
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
                    width: '70%',
                    maxWidth: '1200px',
                    minWidth: '50%',
                    backgroundColor: theme.palette.background.paper,
                    backgroundImage: 'none',
                },
            }}
        >
            <Grid container spacing={2} justifyContent='center' alignItems='center'>
                <Grid item xs={8}>
                    <DialogTitle
                        sx={{
                        color: theme.palette.primary.main,
                        pl: '1.5rem',
                        pt: '1.5rem',
                        fontWeight: '700',
                        fontSize: '1.2rem',
                        }}
                    >
                        Entry
                    </DialogTitle>
                </Grid>
                <Grid item xs={4}>
                    <Box display='flex' justifyContent='flex-end'>
                        <IconButton
                            aria-label='close'
                            onClick={onClose}
                            sx={{
                                color: theme.palette.primary.main,
                                mt: '0.5rem',
                                mr: '1.5rem',
                            }}
                        >
                            <CloseIcon />
                        </IconButton>
                    </Box>
                </Grid>
            </Grid>
                <Card sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'space-between', 
                    height: 'auto',
                    p: '0.5rem',
                    pb: '0rem',
                    minHeight: '10rem',
                    overflowY: 'auto',
                }}>
                <CardContent>
                    <Grid container spacing={2}>
                        <Grid item xs={10} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}>
                            <Grid item xs={12} sx={{ display: 'flex', flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', maxHeight: '5vh', borderBottom: '3px solid black',  width: '100%' }}>
                                <Typography sx={textStyle}>{entry?.fullName}</Typography>
                                <Typography sx={textStyle}>{entry?.name}</Typography>
                                <Typography sx={textStyle}>{entry?.modifyDate.split('T')[0]}</Typography>
                            </Grid>
                            <Grid item xs={12} >
                            <ReactMarkdown
                                rehypePlugins={[rehypeHighlight]}
                                components={{
                                    h1: forwardRef(({ node, ...props}, ref) =>  (<Typography variant="h4"  gutterBottom {...props} sx={{ fontWeight: 'bold' }} ref={ref} />)),
                                    h2: forwardRef(({ node, ...props}, ref) =>  (<Typography variant="h5"  gutterBottom {...props} sx={{ fontWeight: 'bold' }} ref={ref} />)),
                                    h3: forwardRef(({ node, ...props}, ref) =>  (<Typography variant="h6"  gutterBottom {...props} sx={{ fontWeight: 'bold' }} ref={ref} />)),
                                    p: forwardRef(({ node, ...props}, ref) => <Typography paragraph {...props}  ref={ref} />),
                                    a: ({ node, ...props }) => <a {...props} style={{ color: theme.palette.primary.main }} />,
                                    ul: ({ node, ...props }) => <ul {...props} style={{ paddingLeft: '20px' }} />,
                                    ol: ({ node, ...props }) => <ol {...props} style={{ paddingLeft: '20px' }} />,
                                    code: ({ node, inline, className, children, ...props } : CodeProps) => {
                                        const match = /language-(\w+)/.exec(className || '');
                                        return !inline && match ? (
                                            <Box
                                                component="pre"
                                                sx={{
                                                    p: 2,
                                                    borderRadius: 1,
                                                    bgcolor: alpha(theme.palette.background.default, 0.6),
                                                    overflow: 'auto',
                                                    fontFamily: 'monospace',
                                                    fontSize: '0.875rem',
                                                }}
                                            >
                                                <code className={className} {...props}>
                                                    {children}
                                                </code>
                                            </Box>
                                        ) : (
                                            <code className={className} {...props} style={{ 
                                                backgroundColor: alpha(theme.palette.background.default, 0.6),
                                                padding: '2px 4px',
                                                borderRadius: '3px',
                                                fontFamily: 'monospace'
                                            }}>
                                                {children}
                                            </code>
                                        );
                                    },
                                }}
                            >
                                {entry?.text}
                            </ReactMarkdown>
                            </Grid>
                                {entry && entry.linkedEntries.length > 0 && (
                                    <Grid item xs={12}>
                                        <Box mt={2}>
                                            <div style={{ fontWeight: 'bold', marginBottom: '8px' }}>Linked Entries:</div>
                                            <Box display="flex" flexWrap="wrap" gap={1}>
                                                {entry.linkedEntries.map((entry) : any => (
                                                    <Chip 
                                                        key={entry.id} 
                                                        label={entry.name}
                                                        color="primary"
                                                        onClick={() => {
                                                            setSelectedEntry(entry.id);
                                                            setOpenEntry(true);
                                                        }}
                                                    />
                                                ))}
                                            </Box>
                                        </Box>
                                    </Grid>
                                )}
                        </Grid>
                        <Grid item xs={2} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '1rem' }}>
                            <IconButton onClick={() => setOpenFileManagment(true)} disabled={!hasFile}>
                                <FileOpenIcon sx={{ fontSize: 80, color: hasFile ? theme.palette.primary.main : 'gray', cursor: hasFile ? 'pointer' : 'auto' }} />
                            </IconButton>
                        </Grid>
                    </Grid>
                </CardContent>
                {entry?.file && (
                <ViewFileCard
                    open={openFileManagment}
                    onClose={() => setOpenFileManagment(false)}
                    fullOwner={false}
                    entry={entry as any}
                />
                )}
                {openEntry && (
                    <ViewEntryCard
                        open={openEntry}
                        onClose={() => {
                            setOpenEntry(false);
                            setSelectedEntry('');
                        }}
                        entryId={selectedEntryId}
                    />
                )}
            </Card>
        </Dialog>
    );
}