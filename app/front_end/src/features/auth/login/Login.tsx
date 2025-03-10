import { Button, Card, DialogActions, DialogContent, Grid, TextField, Typography, useTheme } from '@mui/material';
import { UserContext } from "@/services/authProvider";
import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import { Formik, Form } from 'formik';
import { loginTemplateValidation } from '@/validation/loginTemplate';
import { login } from '@/services/api';
import { Link } from 'react-router-dom';
import toastService from '@/services/toast';


function Login(){

  const navigate = useNavigate();
  const user = useContext(UserContext);
  const Theme = useTheme();

  const initialValues = {
    email: '',
    password: '',
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
          Login
        </Typography>
      <Formik
        initialValues={initialValues}
        onSubmit={async (values) => {

          const request = {
            username: values.email,
            password: values.password,
          };

          const response = await login(request);
          if (response !== undefined)
            toastService.success('Login successful');
          user?.login(response?.token);

          navigate('/');
        }}
        validationSchema={loginTemplateValidation}
      >
        {({ values, handleChange, handleBlur, errors, touched, isSubmitting }) => (
          <Form>
            <Card sx={{ p: 2, mt: 2}}>
              <DialogContent >
                <Grid container spacing={1} >
                  <Grid item xs={12} style={{ fontWeight: 'bold' }} >Email:</Grid>
                  <Grid item xs={12}>
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
                        color: Theme.palette.primary.main,
                        fontSize: '0.875rem',
                        fontWeight: 'bold',
                      }}
                    />
                  </Grid>
                </Grid>
                <Link to='/register' replace style={{
                  color: Theme.palette.primary.main,
                  fontWeight: 'bold',
                  fontSize: '0.875rem',
                  textDecoration: 'none',
                  display: 'flex',
                  justifyContent: 'center',
                  marginTop: '1rem',
                }}>
                  Need an account? Register here.
                </Link>
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
                }}
              >
                Login
              </Button>
              </DialogActions>
            </Card>
          </Form>
        )}
      </Formik>
    </>
  );
}

export default Login;