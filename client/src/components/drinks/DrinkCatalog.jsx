import React, { useState, useEffect } from 'react';
import {
  Container,
  Grid,
  Card,
  CardMedia,
  CardContent,
  Typography,
  IconButton,
  Badge,
  AppBar,
  Toolbar,
  Box,
  Select,
  MenuItem,
  Slider,
  FormControl,
  InputLabel,
  Divider,
  Modal,
  List,
  ListItem,
  ListItemText,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControlLabel,
  Checkbox
} from '@mui/material';
import { Add, Remove, ShoppingCart, Close, ArrowBack, CheckCircle, Cancel } from '@mui/icons-material';
import { HubConnectionBuilder } from '@microsoft/signalr';

const DrinkCatalog = () => {
  const [drinks, setDrinks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [selectedBrand, setSelectedBrand] = useState('All');
  const [priceRange, setPriceRange] = useState([0, 1000]);
  const [hideUnavailable, setHideUnavailable] = useState(false);
  const [sortOrder, setSortOrder] = useState('default');

  const getFilteredDrinks = () => {
    let result = drinks;

    if (selectedBrand !== 'All') {
      result = result.filter(drink => drink.brandName === selectedBrand);
    }

    result = result.filter(drink =>
      drink.price >= priceRange[0] && drink.price <= priceRange[1]
    );

    if (hideUnavailable) {
      result = result.filter(drink => drink.amount > 0);
    }

    switch (sortOrder) {
      case 'priceAsc':
        result = [...result].sort((a, b) => a.price - b.price);
        break;
      case 'priceDesc':
        result = [...result].sort((a, b) => b.price - a.price);
        break;
      case 'nameAsc':
        result = [...result].sort((a, b) => a.name.localeCompare(b.name));
        break;
      case 'amountAsc':
        result = [...result].sort((a, b) => a.amount - b.amount);
        break;
      case 'amountDesc':
        result = [...result].sort((a, b) => b.amount - a.amount);
        break;
      default:
        break;
    }

    return result;
  };

  const [cart, setCart] = useState({});
  const [cartOpen, setCartOpen] = useState(false);

  const [paymentOpen, setPaymentOpen] = useState(false);
  const [coinQuantities, setCoinQuantities] = useState({ 1: 0, 2: 0, 5: 0, 10: 0 });
  const [amountInserted, setAmountInserted] = useState(0);
  const [paymentStatus, setPaymentStatus] = useState(null);
  const [change, setChange] = useState(null);
  const [connectionError, setConnectionError] = useState(null);

  useEffect(() => {
    let connection;
    const startConnection = async () => {
      connection = new HubConnectionBuilder()
        .withUrl("https://localhost:7223/vendingMachineHub?clientId=user")
        .build();
      console.log("Connect");
      connection.on("ConnectionRejected", (reason) => {
        setConnectionError(reason);
      });

      try {
        await connection.start();
        setConnectionError(null);
      } catch (err) {
        if (err.message.includes("already connected")) {
          setConnectionError("This device is already connected in another tab.");
        } else {
          setConnectionError("Connection failed. Please refresh the page.");
        }
      }
    };
    startConnection();
    fetchDrinks();
    return () => {
      if (connection) connection.stop();
    };
  }, []);

  useEffect(() => {
    const calculatedAmount = Object.entries(coinQuantities).reduce(
      (sum, [denomination, quantity]) => sum + (Number(denomination) * quantity), 0
    );
    setAmountInserted(calculatedAmount);
  }, [coinQuantities]);

  const addToCart = (drinkId) => {
    setCart(prev => ({
      ...prev,
      [drinkId]: (prev[drinkId] || 0) + 1
    }));
  };

  const removeFromCart = (drinkId) => {
    setCart(prev => {
      const newQuantity = (prev[drinkId] || 0) - 1;
      if (newQuantity <= 0) {
        const newCart = { ...prev };
        delete newCart[drinkId];
        return newCart;
      }
      return { ...prev, [drinkId]: newQuantity };
    });
  };

  const removeItemCompletely = (drinkId) => {
    setCart(prev => {
      const newCart = { ...prev };
      delete newCart[drinkId];
      return newCart;
    });
  };

  const getCartItemCount = () => {
    return Object.values(cart).reduce((sum, quantity) => sum + quantity, 0);
  };

  const getCartItems = () => {
    return Object.entries(cart).map(([id, quantity]) => {
      const drink = drinks.find(d => d.id === parseInt(id));
      return drink ? {
        ...drink,
        brandName: drink.brand?.name || drink.brandName,
        quantity
      } : null;
    }).filter(Boolean);
  };

  const getTotalPrice = () => {
    return getCartItems().reduce((total, item) =>
      total + (item.price * item.quantity), 0);
  };

  const handleCoinChange = (denomination, delta) => {
    setCoinQuantities(prev => ({
      ...prev,
      [denomination]: Math.max(0, prev[denomination] + delta)
    }));
  };

  const fetchDrinks = async () => {
    try {
      const response = await fetch('https://localhost:7223/api/Drinks');
      if (!response.ok) throw new Error('Failed to fetch drinks');
      const data = await response.json();

      const transformedDrinks = data.map(drink => ({
        ...drink,
        brandName: drink.brand?.name || 'Unknown Brand'
      }));

      setDrinks(transformedDrinks);

      const prices = data.map(drink => drink.price);
      setPriceRange([Math.min(...prices), Math.max(...prices)]);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handlePayment = async () => {
    const paymentData = {
      items: getCartItems().map(item => ({
        drinkName: item.name,
        brandName: item.brand?.name || item.brandName,
        priceAtPurchase: item.price,
        quantity: item.quantity
      })),
      coins: Object.entries(coinQuantities)
        .filter(([_, qty]) => qty > 0)
        .map(([denomination, quantity]) => ({
          denomination: parseInt(denomination),
          quantity
        }))
    };

    try {
      const response = await fetch('https://localhost:7223/api/Payments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(paymentData)
      });

      const result = await response.json();

      if (!response.ok) {
        throw new Error(result.message || 'Payment failed');
      }

      setPaymentStatus('success');
      setChange(result.change);
      setCart({});

      await fetchDrinks();

    } catch (error) {
      setPaymentStatus('failed');
    }
  };

  const resetPayment = () => {
    setCoinQuantities({ 1: 0, 2: 0, 5: 0, 10: 0 });
    setPaymentStatus(null);
    setChange(null);
  };

  const brands = ['All', ...new Set(drinks.map(drink => drink.brand?.name || 'Unknown Brand'))];

  if (loading) return <Typography>Loading drinks...</Typography>;
  if (error) return <Typography color="error">Error: {error}</Typography>;


  if (connectionError) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        flexDirection: 'column',
        padding: 20,
        textAlign: 'center'
      }}>
        <Typography variant="h5" color="error" gutterBottom>
          Connection Rejected
        </Typography>
        <Typography variant="body1" sx={{ mb: 3 }}>
          {connectionError}
        </Typography>
        <Button
          variant="contained"
          onClick={() => window.location.reload()}
        >
          Try Again
        </Button>
      </div>
    );
  }

  return (
    <>

      {/* App Bar */}
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Drink Vending Machine
          </Typography>
          <IconButton color="inherit" onClick={() => setCartOpen(true)}>
            <Badge badgeContent={getCartItemCount()} color="secondary">
              <ShoppingCart />
            </Badge>
          </IconButton>
        </Toolbar>
      </AppBar>

      {/* Main Content */}
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        {/* Filter Section*/}
        <Box sx={{ mb: 4, p: 3, bgcolor: 'background.paper', borderRadius: 2, boxShadow: 1 }}>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, alignItems: 'center' }}>
            <FormControl sx={{ minWidth: 140 }}>
              <InputLabel>Brand</InputLabel>
              <Select
                value={selectedBrand}
                label="Brand"
                onChange={(e) => setSelectedBrand(e.target.value)}
                MenuProps={{ PaperProps: { sx: { maxHeight: 300 } } }}
              >
                {brands.map(brand => (
                  <MenuItem key={brand} value={brand}>{brand}</MenuItem>
                ))}
              </Select>
            </FormControl>

            <Box sx={{ minWidth: 250, flexGrow: 1 }}>
              <Typography gutterBottom sx={{ mb: 1 }}>
                Price: {priceRange[0]} - {priceRange[1]}
              </Typography>
              <Slider
                value={priceRange}
                onChange={(e, newValue) => setPriceRange(newValue)}
                valueLabelDisplay="auto"
                min={0}
                max={Math.ceil(Math.max(...drinks.map(d => d.price)))}
                step={10}
                size="small"
              />
            </Box>

            <FormControl sx={{ minWidth: 180 }}>
              <InputLabel>Sort By</InputLabel>
              <Select
                value={sortOrder}
                label="Sort By"
                onChange={(e) => setSortOrder(e.target.value)}
                MenuProps={{ PaperProps: { sx: { maxHeight: 300 } } }}
              >
                <MenuItem value="default">Default</MenuItem>
                <MenuItem value="priceAsc">Price: Low to High</MenuItem>
                <MenuItem value="priceDesc">Price: High to Low</MenuItem>
                <MenuItem value="nameAsc">Name: A-Z</MenuItem>
                <MenuItem value="amountAsc">Amount: Low to High</MenuItem>
                <MenuItem value="amountDesc">Amount: High to Low</MenuItem>
              </Select>
            </FormControl>

            <FormControlLabel
              control={
                <Checkbox
                  checked={hideUnavailable}
                  onChange={(e) => setHideUnavailable(e.target.checked)}
                />
              }
              label="Hide Unavailable"
              sx={{ ml: 'auto' }}
            />
          </Box>
        </Box>

        {/* Drink Grid */}
        <Grid container spacing={3} justifyContent="center">
          {getFilteredDrinks().map(drink => (
            <Grid item key={drink.id} xs={12} sm={6} md={4} lg={3} sx={{ display: 'flex', justifyContent: 'center' }}>
              <Card
                sx={{
                  width: 260,
                  height: 400,
                  display: 'flex',
                  flexDirection: 'column',
                  justifyContent: 'space-between',
                  position: 'relative',
                  boxShadow: 3
                }}
              >
                <CardMedia
                  component="img"
                  height="180"
                  image={drink.imageUrl ? `/static/${drink.imageUrl}` : '/static/default-drink.png'}
                  alt={drink.name}
                  sx={{
                    objectFit: 'contain',
                    p: 1,
                    width: '100%'
                  }}
                />
                <CardContent sx={{ flexGrow: 1 }}>
                  <Typography
                    gutterBottom
                    variant="h6"
                    component="div"
                    sx={{
                      wordBreak: 'break-word',
                      display: '-webkit-box',
                      WebkitLineClamp: 2,
                      WebkitBoxOrient: 'vertical',
                      overflow: 'hidden'
                    }}
                  >
                    {drink.name}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {drink.brandName}
                  </Typography>
                </CardContent>
                <Box sx={{
                  px: 2,
                  pb: 2,
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center'
                }}>
                  <Typography variant="h6" color="primary">
                    {drink.price.toFixed(2)}
                  </Typography>
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <IconButton
                      color="error"
                      onClick={() => removeFromCart(drink.id)}
                      disabled={!cart[drink.id]}
                      size="small"
                    >
                      <Remove fontSize="small" />
                    </IconButton>
                    <Typography component="span" sx={{ mx: 1, minWidth: '20px', textAlign: 'center' }}>
                      {cart[drink.id] || 0}
                    </Typography>
                    <IconButton
                      color="success"
                      onClick={() => addToCart(drink.id)}
                      disabled={drink.amount <= (cart[drink.id] || 0)}
                      size="small"
                    >
                      <Add fontSize="small" />
                    </IconButton>
                  </Box>
                </Box>
                {drink.amount <= 0 && (
                  <Box sx={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    backgroundColor: 'rgba(0,0,0,0.5)',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center'
                  }}>
                    <Typography variant="h6" color="white">
                      Out of Stock
                    </Typography>
                  </Box>
                )}
              </Card>
            </Grid>
          ))}
        </Grid>
      </Container>


      {/* Shopping Cart Modal */}
      <Modal
        open={cartOpen}
        onClose={() => setCartOpen(false)}
        aria-labelledby="shopping-cart-modal"
      >
        <Box sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: { xs: '90%', sm: '80%', md: '60%' },
          maxWidth: '800px',
          maxHeight: '80vh',
          bgcolor: 'background.paper',
          boxShadow: 24,
          borderRadius: 2,
          overflow: 'hidden',
          display: 'flex',
          flexDirection: 'column'
        }}>
          <Box sx={{
            p: 2,
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            borderBottom: '1px solid',
            borderColor: 'divider'
          }}>
            <Typography variant="h6">Your Shopping Cart</Typography>
            <IconButton onClick={() => setCartOpen(false)}>
              <Close />
            </IconButton>
          </Box>

          <Box sx={{ overflowY: 'auto', flex: 1 }}>
            {getCartItems().length === 0 ? (
              <Box sx={{ p: 4, textAlign: 'center' }}>
                <Typography variant="body1">Your cart is empty</Typography>
              </Box>
            ) : (
              <List>
                {getCartItems().map(item => (
                  <ListItem
                    key={item.id}
                    sx={{
                      borderBottom: '1px solid',
                      borderColor: 'divider',
                      py: 2
                    }}
                  >
                    <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
                      <CardMedia
                        component="img"
                        image={`/static/${item.imageUrl}`}
                        alt={item.name}
                        sx={{
                          width: 60,
                          height: 60,
                          objectFit: 'contain',
                          mr: 2
                        }}
                      />
                      <Box sx={{ flex: 1 }}>
                        <Typography variant="subtitle1">{item.name}</Typography>
                        <Typography variant="body2" color="text.secondary">
                          {item.brandName}
                        </Typography>
                      </Box>
                      <Box sx={{ display: 'flex', alignItems: 'center', mr: 2 }}>
                        <IconButton
                          size="small"
                          onClick={() => removeFromCart(item.id)}
                        >
                          <Remove fontSize="small" />
                        </IconButton>
                        <Typography sx={{ mx: 1 }}>{item.quantity}</Typography>
                        <IconButton
                          size="small"
                          onClick={() => addToCart(item.id)}
                          disabled={item.amount <= item.quantity}
                        >
                          <Add fontSize="small" />
                        </IconButton>
                      </Box>
                      <Typography sx={{ minWidth: '80px', textAlign: 'right' }}>
                        {(item.price * item.quantity).toFixed(2)}
                      </Typography>
                      <IconButton
                        size="small"
                        onClick={() => removeItemCompletely(item.id)}
                        sx={{ ml: 1 }}
                      >
                        <Close fontSize="small" />
                      </IconButton>
                    </Box>
                  </ListItem>
                ))}
              </List>
            )}
          </Box>

          {getCartItems().length > 0 && (
            <Box sx={{
              p: 2,
              borderTop: '1px solid',
              borderColor: 'divider',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}>
              <Typography variant="h6">Total: {getTotalPrice().toFixed(2)}</Typography>
              <Button
                variant="contained"
                color="primary"
                size="large"
                onClick={() => {
                  setCartOpen(false);
                  setPaymentOpen(true);
                }}
              >
                Purchase
              </Button>
            </Box>
          )}
        </Box>
      </Modal>

      {/* Payment Modal */}
      <Dialog
        open={paymentOpen}
        onClose={() => {
          setPaymentOpen(false);
          resetPayment();
        }}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle>
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <IconButton onClick={() => {
              setPaymentOpen(false);
              resetPayment();
            }} sx={{ mr: 2 }}>
              <ArrowBack />
            </IconButton>
            Payment
          </Box>
        </DialogTitle>
        <DialogContent>
          <Box sx={{ mb: 3 }}>
            <Typography variant="h6" gutterBottom>
              Order Summary
            </Typography>
            <List dense>
              {getCartItems().map(item => (
                <ListItem key={item.id}>
                  <ListItemText
                    primary={`${item.name} (x${item.quantity})`}
                    secondary={item.brandName}
                  />
                  <Typography>{(item.price * item.quantity).toFixed(2)}</Typography>
                </ListItem>
              ))}
            </List>
            <Divider sx={{ my: 2 }} />
            <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
              <Typography variant="h6">Total:</Typography>
              <Typography variant="h6">{getTotalPrice().toFixed(2)}</Typography>
            </Box>
          </Box>

          <Box sx={{ mb: 3 }}>
            <Typography variant="h6" gutterBottom>
              Insert Coins
            </Typography>
            <List>
              {[10, 5, 2, 1].map(denomination => (
                <ListItem key={denomination}>
                  <ListItemText primary={`${denomination} coin`} />
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <IconButton
                      onClick={() => handleCoinChange(denomination, -1)}
                      disabled={coinQuantities[denomination] <= 0}
                    >
                      <Remove />
                    </IconButton>
                    <Typography sx={{ mx: 2 }}>
                      {coinQuantities[denomination]}
                    </Typography>
                    <IconButton onClick={() => handleCoinChange(denomination, 1)}>
                      <Add />
                    </IconButton>
                  </Box>
                </ListItem>
              ))}
            </List>
            <Divider sx={{ my: 2 }} />
            <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
              <Typography variant="h6">Amount Inserted:</Typography>
              <Typography variant="h6">{amountInserted.toFixed(2)}</Typography>
            </Box>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
              <Typography variant="h6">Remaining:</Typography>
              <Typography
                variant="h6"
                color={amountInserted >= getTotalPrice() ? 'success.main' : 'error.main'}
              >
                {Math.max(0, getTotalPrice() - amountInserted).toFixed(2)}
              </Typography>
            </Box>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button
            variant="outlined"
            color="secondary"
            onClick={() => {
              setPaymentOpen(false);
              resetPayment();
            }}
            startIcon={<Cancel />}
          >
            Cancel
          </Button>
          <Button
            variant="contained"
            color="primary"
            onClick={handlePayment}
            disabled={amountInserted < getTotalPrice()}
            startIcon={<CheckCircle />}
          >
            Pay Now
          </Button>
        </DialogActions>

        {/* Payment Success Dialog */}
        <Dialog open={paymentStatus === 'success'} fullWidth maxWidth="sm">
          <DialogTitle>Payment Successful!</DialogTitle>
          <DialogContent>
            <Typography gutterBottom>
              Thank you for your purchase! Please take your change: {change?.amount?.toFixed(2)}
            </Typography>
            {change?.coins?.length > 0 && (
              <Box sx={{ mt: 2 }}>
                <Typography variant="subtitle1">Change in coins:</Typography>
                <List dense>
                  {change.coins.map((coin, index) => (
                    <ListItem key={index}>
                      <ListItemText primary={`${coin.denomination} - ${coin.quantity} coins`} />
                    </ListItem>
                  ))}
                </List>
              </Box>
            )}
          </DialogContent>
          <DialogActions>
            <Button
              onClick={() => {
                setPaymentStatus(null);
                setPaymentOpen(false);
                resetPayment();
                fetchDrinks();
              }}
              variant="contained"
              color="primary"
            >
              Back to Catalog
            </Button>
          </DialogActions>
        </Dialog>

        {/* No Change Available Dialog */}
        <Dialog open={paymentStatus === 'no_change'}>
          <DialogTitle>Sorry</DialogTitle>
          <DialogContent>
            <Typography>
              We cannot complete your purchase because the machine cannot provide the exact change needed.
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button
              onClick={() => setPaymentStatus(null)}
              color="primary"
            >
              OK
            </Button>
          </DialogActions>
        </Dialog>

        {/* Payment Failed Dialog */}
        <Dialog open={paymentStatus === 'failed'}>
          <DialogTitle>Payment Failed</DialogTitle>
          <DialogContent>
            <Typography>
              There was an error processing your payment. Please try again.
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button
              onClick={() => setPaymentStatus(null)}
              color="primary"
            >
              OK
            </Button>
          </DialogActions>
        </Dialog>
      </Dialog>
    </>
  );
};

export default DrinkCatalog;