import { Button, Card, DialogContent, DialogActions, Grid, TextField, Typography, useTheme } from '@mui/material';
import { Formik, Form } from 'formik';
import { registerTemplate } from '@/validation/registerTemplate';
import { useRegister } from '@/hooks/user';

export default function Register() {

    const register = useRegister();
    const Theme = useTheme();

  
    const initialValues = {
      password: '',
      email: '',
      name: '',
      surname: '',
    };
  
    return (
      <>
        <Typography sx={{ 
          mt: '0.5rem', 
          fontSize: '2.5rem', 
          fontWeight: 'bold', 
          color: Theme.palette.primary.main,
          display: 'flex',
          justifyContent: 'center',
        }}>
          Register
        </Typography>
        <Formik
          initialValues={initialValues}
          onSubmit={async (values) => {
            register.mutateAsync(values);
          }}
          validationSchema={registerTemplate}
        >
          {({ values, handleChange, handleBlur, errors, touched, isSubmitting }) => (
            <Form>
              <Card style={{background: '#EFFCFF'}}>
                <DialogContent>
                  <Grid container rowSpacing={1} spacing={1}>
                    <Grid item xs={12} style={{ fontWeight: 'bold' }}>Email:</Grid>
                    <Grid item xs={12} >
                      <TextField
                        name="email"
                        label="Email"
                        value={values.email}
                        onChange={handleChange}
                        onBlur={handleBlur}
                        variant="outlined"
                        fullWidth
                        error={Boolean(errors.email && touched.email)}
                        helperText={errors.email && touched.email && errors.email}
                        sx={{
                          color: Theme.palette.primary.contrastText,
                          fontSize: '0.875rem',
                          fontWeight: 'bold',
                        }}
                      />
                    </Grid>
                    <Grid item xs={12} style={{ fontWeight: 'bold' }} >Name:</Grid>
                    <Grid item xs={12} >
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
                    <Grid item xs={12} style={{ fontWeight: 'bold' }}>Surname:</Grid>
                    <Grid item xs={12} >
                      <TextField
                        name="surname"
                        label="Surname"
                        value={values.surname}
                        onChange={handleChange}
                        onBlur={handleBlur}
                        variant="outlined"
                        fullWidth
                        error={Boolean(errors.surname && touched.surname)}
                        helperText={errors.surname && touched.surname && errors.surname}
                        sx={{
                          color: Theme.palette.primary.contrastText,
                          fontSize: '0.875rem',
                          fontWeight: 'bold',
                        }}
                      />
                    </Grid>
                    <Grid item xs={12} style={{ fontWeight: 'bold' }} >Password:</Grid>
                    <Grid item xs={12} >
                      <TextField
                        name="password"
                        type="password"
                        label="Password"
                        value={values.password}
                        onChange={handleChange}
                        onBlur={handleBlur}
                        variant="outlined"
                        fullWidth 
                        error={Boolean(errors.password && touched.password)}
                        helperText={errors.password && touched.password && errors.password}
                        sx={{
                          color: Theme.palette.primary.contrastText,
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
                      fontSize: '0.875rem',
                      margin: 'auto',
                      mb: '1rem',
                    }}
                  >
                    Register
                  </Button>
                </DialogActions>
              </Card>
            </Form>
          )}
        </Formik>
      </>
    );
  };