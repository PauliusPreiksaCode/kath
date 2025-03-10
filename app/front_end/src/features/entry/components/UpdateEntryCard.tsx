import { alpha, Box, Dialog, DialogContent, DialogActions, Button, DialogTitle, TextField, Grid, IconButton, useTheme } from '@mui/material';
import { EntryProps } from "../EntryList";
import { useUpdateEntry } from '@/hooks/entry';
import { useContext, useState } from 'react';
import { OrganizationContext } from '@/services/organizationProvider';
import { Formik, Form } from 'formik';
import { Close as CloseIcon } from '@mui/icons-material';
import { EntryTemplateValidation } from "@/validation/entryTemplate";
import FileInput from "./FileInput";
import { fileService } from "@/services/fileService";

interface UpdateEntryCardProps {
    open: boolean;
    onClose: () => void;
    entry: EntryProps;
};

export default function UpdateEntryCard({ open, onClose, entry }: UpdateEntryCardProps) {

    const Theme = useTheme();
    const updateEntry = useUpdateEntry();
    const organizationContext = useContext(OrganizationContext);
    const [file, setFile] = useState<string | ArrayBuffer | null>(null);
    const [fileName, setFileName] = useState<string>('');

    const initialValues = {
        entryName: entry.name,
        text: entry.text,
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
                        Update Entry
                    </DialogTitle>
                </Grid>
                <Grid item xs={4}>
                    <Box display='flex' justifyContent='flex-end'>
                        <IconButton
                            aria-label='close'
                            onClick={() => {
                                setFile(null);
                                setFileName('');
                                onClose();
                            }}
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

                        const request = {
                            entryId: entry.id,
                            entryName: values.entryName, 
                            text: values.text,
                            groupId: organizationContext.groupId,
                            organizationId: organizationContext.organizationId,
                            file: binaryData,
                            name: name,
                            extension: `.${extension}`,
                        };

                        await updateEntry.mutateAsync(request);
                        setFile(null);
                        setFileName('');
                        onClose();
                    }}
                    validationSchema={EntryTemplateValidation}
                >
                    {({ values, handleChange, handleBlur, errors, touched, isSubmitting }) => (
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
                                    <Grid item xs={12} style={{ fontWeight: 'bold' }} >Text:</Grid>
                                    <Grid item xs={12}>
                                        <TextField
                                            name="text"
                                            label="Text"
                                            value={values.text}
                                            onChange={handleChange}
                                            onBlur={handleBlur}
                                            variant="outlined"
                                            fullWidth
                                            multiline
                                            rows={4}
                                            error={Boolean(errors.text && touched.text)}
                                            helperText={errors.text && touched.text && errors.text}
                                            sx={{
                                                color: Theme.palette.primary.contrastText,
                                                fontSize: '0.875rem',
                                                fontWeight: 'bold',
                                            }}
                                        />
                                    </Grid>
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
                        </Form>
                    )}
                </Formik>
            </DialogContent>
        </Dialog>
    );
};