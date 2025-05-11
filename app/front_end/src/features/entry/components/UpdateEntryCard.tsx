import { alpha, Box, Dialog, DialogContent, Autocomplete, DialogActions, Chip, Button, DialogTitle, TextField, Tabs, Tab, Paper, Grid, IconButton, useTheme, CircularProgress, Typography } from '@mui/material';
import { EntryProps } from "../EntryList";
import { useUpdateEntry, useGetLinkingEntries } from '@/hooks/entry';
import { useContext, useState, useEffect, forwardRef } from 'react';
import { OrganizationContext } from '@/services/organizationProvider';
import { Formik, Form } from 'formik';
import { Close as CloseIcon, AutoAwesome as AIIcon } from '@mui/icons-material';
import { EntryTemplateValidation } from "@/validation/entryTemplate";
import FileInput from "./FileInput";
import { fileService } from "@/services/fileService";
import useEntryLinking, { LinkedentryProps } from "@/services/entryLinkingService";
import ReactMarkdown from 'react-markdown';
import rehypeHighlight from 'rehype-highlight';
import 'highlight.js/styles/github.css';
import { CodeProps, MarkdownHelperText } from './CreateEntryCard';
import ViewEntryCard from './ViewEntryCard';
import SuggestionCard from './SuggestionCard';

interface UpdateEntryCardProps {
    open: boolean;
    onClose: () => void;
    entry: EntryProps;
};

export default function UpdateEntryCard({ open, onClose, entry }: UpdateEntryCardProps) {

    const Theme = useTheme();
    const organizationContext = useContext(OrganizationContext);
    const updateEntry = useUpdateEntry(organizationContext.organizationId);
    const [file, setFile] = useState<string | ArrayBuffer | null>(null);
    const [fileName, setFileName] = useState<string>('');
    const [availableEntries, setAvailableEntries] = useState<LinkedentryProps[]>([]);
    const linkingEntries = useGetLinkingEntries(organizationContext.organizationId, entry.id);
    const [tabValue, setTabValue] = useState(0);
    const [selectedEntryId, setSelectedEntry] = useState<string>('');
    const [openEntry, setOpenEntry] = useState<boolean>(false);
    const [openSuggestions, setOpenSuggestions] = useState<boolean>(false);
    
    const linking = useEntryLinking();

    useEffect(() => {
            setAvailableEntries(linkingEntries.data || []);
    }, [linkingEntries.data]);

    useEffect(() => {
        if(open && entry) {
            linking.initializeLinks(entry.text, availableEntries);
            setFile(null);
            setFileName('');
            linking.setShowAutocomplete(false);
            setTabValue(0);
        }
    }, [open]);

    if(linkingEntries.isLoading && !linkingEntries.isFetching) {
        return (
            <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100vh" }}>
              <CircularProgress />
            </Box>
        );
    }

    const initialValues = {
        entryName: entry.name,
        text: entry.text,
    };

    const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    return (
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
                    backgroundColor: Theme.palette.background.paper,
                    backgroundImage: 'none',
                    width: '70%',
                    maxWidth: '1200px',
                    minWidth: '50%',
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
                        ref={linking.textBoxRef}
                    >
                        Update Entry
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
                <Formik
                    initialValues={initialValues}
                    onSubmit={async (values) => {

                        const binaryData = file !== null ? fileService.fileToBlob(file) : null;
                        const [name, extension] = fileName?.split('.');

                        const extractedEntries = linking.extractLinkedEntries(values.text, availableEntries);
                        const linkedEntriesIds = extractedEntries.map(e => e.id);

                        const request = {
                            entryId: entry.id,
                            entryName: values.entryName, 
                            text: values.text,
                            groupId: organizationContext.groupId,
                            organizationId: organizationContext.organizationId,
                            file: binaryData,
                            name: name,
                            extension: `.${extension}`,
                            linkedEntries: linkedEntriesIds,
                        };

                        await updateEntry.mutateAsync(request);
                        onClose();
                    }}
                    validationSchema={EntryTemplateValidation}
                >
                    {({ values, handleChange, handleBlur, errors, touched, isSubmitting, setFieldValue }) => (
                        <Form>
                            <DialogContent >
                                <Grid container spacing={1} >
                                    <Grid item xs={12} style={{ fontWeight: 'bold' }} >Name:</Grid>
                                    <Grid item xs={12}>
                                        <TextField
                                            name="entryName"
                                            label="Name"
                                            value={values.entryName}
                                            onChange={handleChange}
                                            onBlur={handleBlur}
                                            variant="outlined"
                                            fullWidth
                                            error={Boolean(errors.entryName && touched.entryName)}
                                            helperText={errors.entryName && touched.entryName && errors.entryName}
                                            sx={{
                                                color: Theme.palette.primary.contrastText,
                                                fontSize: '0.875rem',
                                                fontWeight: 'bold',
                                            }}
                                        />
                                    </Grid>
                                    <Grid item xs={12} style={{ fontWeight: 'bold', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                        <div>Text:</div>
                                        <Tabs 
                                            value={tabValue} 
                                            onChange={handleTabChange} 
                                            aria-label="markdown tabs"
                                            sx={{ minHeight: '36px' }}
                                        >
                                            <Tab label="Edit" sx={{ minHeight: '36px', py: 0 }} />
                                            <Tab label="Preview" sx={{ minHeight: '36px', py: 0 }} />
                                        </Tabs>
                                    </Grid>
                                    <Grid item xs={12}>
                                    {tabValue === 0 ? (
                                        <>
                                        <TextField
                                            name="text"
                                            label="Text"
                                            value={values.text}
                                            onChange={(e) => linking.handleTextChange(e, handleChange)}
                                            onBlur={handleBlur}
                                            variant="outlined"
                                            fullWidth
                                            multiline
                                            rows={10}
                                            error={Boolean(errors.text && touched.text)}
                                            helperText={errors.text && touched.text && errors.text}
                                            sx={{
                                                color: Theme.palette.primary.contrastText,
                                                fontSize: '0.875rem',
                                                fontWeight: 'bold',
                                            }}
                                            ref={linking.textFieldRef}
                                        />
                                        <MarkdownHelperText/>
                                        </>
                                        ) : (
                                            <Paper
                                            elevation={1}
                                            sx={{
                                                p: 2,
                                                minHeight: '200px',
                                                border: '1px solid',
                                                borderColor: 'divider',
                                                borderRadius: 1,
                                                bgcolor: 'background.paper',
                                                overflow: 'auto',
                                                maxHeight: '250px',
                                            }}
                                        >
                                                {values.text ? (
                                                    <ReactMarkdown
                                                        rehypePlugins={[rehypeHighlight]}
                                                        components={{
                                                            h1: forwardRef(({ node, ...props}, ref) =>  (<Typography variant="h4"  gutterBottom {...props} sx={{ fontWeight: 'bold' }} ref={ref} />)),
                                                            h2: forwardRef(({ node, ...props}, ref) =>  (<Typography variant="h5"  gutterBottom {...props} sx={{ fontWeight: 'bold' }} ref={ref} />)),
                                                            h3: forwardRef(({ node, ...props}, ref) =>  (<Typography variant="h6"  gutterBottom {...props} sx={{ fontWeight: 'bold' }} ref={ref} />)),
                                                            p: forwardRef(({ node, ...props}, ref) => <Typography paragraph {...props}  ref={ref} />),
                                                            a: ({ node, ...props }) => <a {...props} style={{ color: Theme.palette.primary.main }} />,
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
                                                                            bgcolor: alpha(Theme.palette.background.default, 0.6),
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
                                                                        backgroundColor: alpha(Theme.palette.background.default, 0.6),
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
                                                        {values.text}
                                                    </ReactMarkdown>
                                                ) : (
                                                    <Typography variant="body2" color="textSecondary" sx={{ fontStyle: 'italic' }}>
                                                        Your formatted content will appear here...
                                                    </Typography>
                                                )}
                                        </Paper>
                                    )}
                                    <Button 
                                        variant="contained" 
                                        onClick={() => setOpenSuggestions(true)} 
                                        sx={{ 
                                            color: Theme.palette.primary.contrastText,
                                            backgroundColor: Theme.palette.primary.main,
                                            fontWeight: 'bold',
                                            fontSize: '1rem',
                                            mt: '0.5rem',
                                            ml: '0.5rem',
                                        }}
                                    >
                                        <span>Suggest related links with AI</span><AIIcon sx={{ ml: 1 }} />
                                    </Button>
                                    {linking.showAutoComplete && (
                                        <Box
                                            sx={{
                                                position: 'absolute',
                                                top: `${linking.autoCompletePosition.top}px`,
                                                left: `${linking.autoCompletePosition.left}px`,
                                                width: 'auto',
                                                minWidth: '18rem',
                                                backgroundColor: Theme.palette.background.paper,
                                                borderRadius: '0.5rem',
                                                height: '10rem',
                                                overflow: 'auto',
                                                padding: '0.5rem',
                                                boxShadow: '0px 0px 10px 0px rgba(0,0,0,0.2)',
                                                border: '1px solid',
                                                borderColor: Theme.palette.text.primary,
                                                zIndex: 100,
                                            }}
                                        >
                                            <Autocomplete
                                                id="entry-autocomplete"
                                                options={availableEntries.filter(e => e.id !== entry.id)}
                                                getOptionLabel={(option) => option.name}
                                                filterOptions={(options, state) => 
                                                    options.filter(option => 
                                                        option.name.toLowerCase().includes(state.inputValue.toLowerCase())
                                                    )
                                                }
                                                renderOption={(props, option) => (
                                                    <li {...props}>
                                                        <div>
                                                            <div style={{ fontWeight: 'bold' }}>{option.name}</div>
                                                            <div style={{ fontSize: '0.8rem' }}>{option.fullName}</div>
                                                        </div>
                                                    </li>
                                                )}
                                                renderInput={(params) => (
                                                    <TextField {...params} label="Search entries" variant="outlined" />
                                                )}
                                                onChange={(_, value) => {
                                                    if (value) {
                                                        linking.insertEntryLink(value, values, setFieldValue);
                                                    }
                                                }}
                                                autoHighlight
                                                open
                                                disablePortal
                                                sx={{ width: '100%' }}
                                            />
                                        </Box>
                                    )}
                                    </Grid>
                                    {linking.linkedEntries.length > 0 && (
                                        <Grid item xs={12}>
                                            <Box mt={2}>
                                                <div style={{ fontWeight: 'bold', marginBottom: '8px' }}>Linked Entries:</div>
                                                <Box display="flex" flexWrap="wrap" gap={1}>
                                                    {linking.linkedEntries.map(entry => (
                                                        <Chip 
                                                            key={entry.id} 
                                                            label={entry.name}
                                                            color="primary"
                                                            onDelete={() => linking.removeEntryLink(entry, values, setFieldValue)}
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
                                    <Grid item xs={12} style={{ fontWeight: 'bold' }} >Change file:</Grid>
                                    <Grid item xs={12}>
                                        <FileInput
                                            setFile={setFile}
                                            setFileName={setFileName}
                                        />
                                        {file && (<div style={{color: 'green'}}>File selected: {fileName}</div>)}
                                    </Grid>
                                </Grid>
                            </DialogContent>
                            <DialogActions>
                                <Button 
                                    type="submit" 
                                    variant="contained" 
                                    disabled={isSubmitting}
                                    sx={{ 
                                        color: Theme.palette.primary.contrastText,
                                        backgroundColor: Theme.palette.primary.main,
                                        fontWeight: 'bold',
                                        fontSize: '1rem',
                                        margin: 'auto',
                                    }}
                                >
                                    Update
                                </Button>
                            </DialogActions>
                            {openSuggestions && (
                                <SuggestionCard
                                    open={openSuggestions}
                                    onClose={() => setOpenSuggestions(false)}
                                    text={values.text}
                                    entryId={entry.id}
                                />
                            )}
                        </Form>
                    )}
                </Formik>
            </DialogContent>
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
        </Dialog>
    );
};