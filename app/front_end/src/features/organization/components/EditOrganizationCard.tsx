import { alpha, Box, Dialog, DialogContent, DialogActions, Button, DialogTitle, TextField, Grid, IconButton, useTheme } from '@mui/material';
import { Close as CloseIcon } from '@mui/icons-material';
import { Formik, Form } from 'formik';
import { OrganizationTemplateValidation } from '@/validation/organizationTemplate';
import { useUpdateOrganization } from '@/hooks/organization';
import { Organization } from '../OrganizationList';


interface EditOrganizationCardProps {
    open: boolean;
    onClose: () => void;
    organization: Organization;
}

export default function EditOrganizationCard ({ open, onClose, organization } : EditOrganizationCardProps)  {

    const Theme = useTheme();
    const editOrganization = useUpdateOrganization(); 

    const initialValues = {
        name: organization.name,
        description: organization.description,
    }

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
                        Edit Organization
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

                        const request = {
                            id: organization.id,
                            name: values.name,
                            description: values.description,
                        }

                        await editOrganization.mutateAsync(request);
                        onClose();
                    }}
                    validationSchema={OrganizationTemplateValidation}
                >
                    {({ values, handleChange, handleBlur, errors, touched, isSubmitting }) => (
                    <Form>
                        <DialogContent >
                            <Grid container spacing={1} >
                                <Grid item xs={12} style={{ fontWeight: 'bold' }} >Name:</Grid>
                                <Grid item xs={12}>
                                    <TextField
                                        name="name"
                                        label="Name"
                                        value={values.name}
                                        onChange={handleChange}
                                        onBlur={handleBlur}
                                        variant="outlined"
                                        fullWidth
                                        error={Boolean(errors.name && touched.name)}
                                        helperText={errors.name && touched.name && errors.name}
                                        sx={{
                                            color: Theme.palette.primary.contrastText,
                                            fontSize: '0.875rem',
                                            fontWeight: 'bold',
                                        }}
                                    />
                                </Grid>
                                <Grid item xs={12} style={{ fontWeight: 'bold' }} >Description:</Grid>
                                <Grid item xs={12} >
                                    <TextField
                                        name="description"
                                        label="Description"
                                        value={values.description}
                                        onChange={handleChange}
                                        onBlur={handleBlur}
                                        variant="outlined"
                                        fullWidth
                                        error={Boolean(errors.description && touched.description)}
                                        helperText={errors.description && touched.description && errors.description}
                                        sx={{
                                            color: Theme.palette.primary.main,
                                            fontSize: '0.875rem',
                                            fontWeight: 'bold',
                                        }}
                                    />
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
}
