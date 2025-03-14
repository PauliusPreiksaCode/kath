import { EntryProps } from "../EntryList";
import { Button, Typography, Card, CardContent, useTheme, Grid, IconButton, alpha, Box } from "@mui/material";
import { forwardRef, useContext, useState } from "react";
import { UserContext } from "@/services/authProvider";
import FileOpenIcon from '@mui/icons-material/FileOpen';
import DeleteEntryCard from "./DeleteEntryCard";
import UpdateEntryCard from "./UpdateEntryCard";
import ViewFileCard from "./ViewFileCard";
import ReactMarkdown from 'react-markdown';
import rehypeHighlight from 'rehype-highlight';
import 'highlight.js/styles/github.css';
import { CodeProps } from "./CreateEntryCard";

export default function EntryCard(entry : EntryProps) {

    const theme = useTheme();
    const userContext = useContext(UserContext);
    const [openEditEntry, setOpenEditEntry] = useState<boolean>(false);
    const [openDeleteEntry, setOpenDeleteEntry] = useState<boolean>(false);
    const [openFileManagment, setOpenFileManagment] = useState<boolean>(false);

    const hasFile = entry.fileId !== null;
    const wordCount = entry.text.trim().split(/\s+/).length;
    
    const textStyle = {
        fontSize: '1.4rem',
        fontWeight: 'bold',
        color: theme.palette.primary.main,
        display: 'flex',
        justifyContent: 'center',
    };

    const buttonStyle ={
        width: '100%',
        fontSize: '1rem', 
        fontWeight: 'bold',
        backgroundColor: theme.palette.primary.main,
        color: theme.palette.primary.contrastText,
        minHeight: '1rem',
        maxHeight: '2.5rem',
    };

    const fullOwner = userContext.userId === entry.licencedUserId;

    return (
        <>
        <Card sx={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between', 
                height: 'auto',
                p: '0.5rem',
                pb: '0rem',
                ":hover": {
                    boxShadow: '0 0 10px 0px rgba(0,0,0,0.2)',
                },
                minHeight: fullOwner ? '10rem' : '8rem',
        }}>
            <CardContent>
                <Grid container spacing={2}>
                    <Grid item xs={10} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}>
                        <Grid item xs={12} sx={{ display: 'flex', flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', maxHeight: '5vh', borderBottom: '3px solid black',  width: '100%' }}>
                            <Typography sx={textStyle}>{entry.fullName}</Typography>
                            <Typography sx={textStyle}>{entry.name}</Typography>
                            <Typography sx={textStyle}>{entry.modifyDate.split('T')[0]}</Typography>
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
                                {entry.text}
                            </ReactMarkdown>
                        </Grid>
                    </Grid>
                    <Grid item xs={2} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '1rem' }}>
                        <IconButton onClick={() => setOpenFileManagment(true)} disabled={!hasFile}>
                            <FileOpenIcon sx={{ fontSize: 80, color: hasFile ? theme.palette.primary.main : 'gray', cursor: hasFile ? 'pointer' : 'auto' }} />
                        </IconButton>
                        { wordCount > 200 && fullOwner && (
                            <>
                                <Grid sx={{ display: 'flex', flexDirection: 'column', width: '90%', gap: '0.5rem' }}>
                                    <Button
                                        variant="contained"
                                        sx={{...buttonStyle}}
                                        onClick={() => setOpenEditEntry(true)}
                                    >
                                            Edit
                                    </Button>
                                    <Button
                                        variant="contained"
                                        sx={{...buttonStyle}}
                                        onClick={() => setOpenDeleteEntry(true)}
                                    >
                                            Delete
                                    </Button>
                                </Grid>
                            </>
                        )}
                        {(wordCount <= 200 && fullOwner) && (
                            <>
                                <Grid item xs={12} sx={{ display: 'flex', flexDirection: 'row', gap: '0.5rem', width: '100%' }}>
                                    <Grid item xs={6}>
                                        <Button
                                            variant="contained"
                                            sx={{...buttonStyle}}
                                            onClick={() => setOpenEditEntry(true)}
                                        >
                                            Edit
                                        </Button>
                                    </Grid>
                                    <Grid item xs={6}>
                                        <Button
                                            variant="contained"
                                            sx={{...buttonStyle}}
                                            onClick={() => setOpenDeleteEntry(true)}
                                        >
                                            Delete
                                        </Button>
                                    </Grid>
                                </Grid>
                            </>
                        )}
                    </Grid>
                </Grid>
            </CardContent>
        </Card>
        {fullOwner && 
        (<> 
            <UpdateEntryCard
                open={openEditEntry}
                onClose={() => setOpenEditEntry(false)}
                entry={entry}
            />
            <DeleteEntryCard
                open={openDeleteEntry}
                onClose={() => setOpenDeleteEntry(false)}
                entry={entry}
            />
        </>)}
        <ViewFileCard
            open={openFileManagment}
            onClose={() => setOpenFileManagment(false)}
            fullOwner={fullOwner}
            entry={entry}
        />
        </>
    );
};