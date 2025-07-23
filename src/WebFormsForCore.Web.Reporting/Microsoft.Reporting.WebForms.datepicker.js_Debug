// <copyright company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
// this function performs an aggressive unicode URL-encoding
// convert non alphanum character into UTF-8 code string
// in format %XX%XX%XX
var UTF8_1ST_OF_2         = 0xc0   ;   // 110x xxxx
var UTF8_1ST_OF_3         = 0xe0   ;   // 1110 xxxx
var UTF8_1ST_OF_4         = 0xf0   ;  // 1111 0xxx
var UTF8_TRAIL            = 0x80   ;   // 10xx xxxx
var HIGH_SURROGATE_BITS   = 0xD800 ;
var LOW_SURROGATE_BITS    = 0xDC00 ;
var SURROGATE_6_BIT       = 0xFC00 ;
var SURROGATE_ID_BITS     = 0xF800 ;
var SURROGATE_OFFSET      = 0x10000;

function escapeProperlyCoreCore(str, bAsUrl, bForFilterQuery, bForCallback)
{
    var strOut = "";
    var strByte = "";
    var ix = 0;
    var strEscaped = " \"%<>\'&";

    if (typeof(str) == "undefined") // making this more robust
        return "";
	
    for (ix = 0; ix < str.length; ix++)
    {
        var charCode = str.charCodeAt(ix);
        var curChar = str.charAt(ix);
        
        if(bAsUrl && (curChar == '#' || curChar == '?') ) 
        {
            strOut += str.substr(ix);
            break;
        }

        if (bForFilterQuery && curChar == '&')
        {
            strOut += curChar;
            continue;
        }

        if (charCode <= 0x7f)
        {
            if (bForCallback)
            {
                strOut += curChar;
            }
            else
            {
                if ( (charCode >= 97 && charCode <= 122) ||
                     (charCode >= 65 && charCode <= 90) ||
                     (charCode >= 48 && charCode <= 57) ||
                     (bAsUrl && (charCode >= 32 && charCode <= 95) && strEscaped.indexOf(curChar) < 0))
                {
                    strOut += curChar;
                }
                else if (charCode <= 0x0f)
                {
                    strOut += "%0" + charCode.toString(16).toUpperCase();
                }
                else if (charCode <= 0x7f)
                {
                    strOut += "%" + charCode.toString(16).toUpperCase();
                }
            }
        }
        else if (charCode <= 0x07ff)
        {
            strByte = UTF8_1ST_OF_2 | (charCode >> 6);
            strOut += "%" + strByte.toString(16).toUpperCase() ; 
            strByte = UTF8_TRAIL | (charCode & 0x003f);
            strOut += "%" + strByte.toString(16).toUpperCase();
        }
        else if ((charCode & SURROGATE_6_BIT) != HIGH_SURROGATE_BITS)
        {
            strByte = UTF8_1ST_OF_3 | (charCode >> 12);
            strOut += "%" + strByte.toString(16).toUpperCase();
            strByte = UTF8_TRAIL | ((charCode & 0x0fc0) >> 6);  // middle 6 bits
            strOut += "%" + strByte.toString(16).toUpperCase();
            strByte = UTF8_TRAIL | (charCode & 0x003f);         // lower 6 bits
            strOut += "%" + strByte.toString(16).toUpperCase();
        }
        else if (ix < str.length - 1)
        {
            var charCode = (charCode & 0x03FF) << 10;           // lower 10 bits of first char
            ix ++; 
            var nextCharCode = str.charCodeAt(ix);

            charCode |= nextCharCode & 0x03FF;                  // lower 10 bits of second char
            charCode += SURROGATE_OFFSET;

            strByte =UTF8_1ST_OF_4 | (charCode >> 18);
            strOut += "%" + strByte.toString(16).toUpperCase();
            strByte =UTF8_TRAIL | ((charCode & 0x3f000) >> 12); // upper 6 bits
            strOut += "%" + strByte.toString(16).toUpperCase();
            strByte =UTF8_TRAIL | ((charCode & 0x0fc0) >> 6);   // middle 6 bits
            strOut += "%" + strByte.toString(16).toUpperCase();
            strByte =UTF8_TRAIL | (charCode & 0x003f);          // lower 6 bits
            strOut += "%" + strByte.toString(16).toUpperCase();
        }
    }
    return strOut;
}

function escapeProperly(str)
{
    return escapeProperlyCoreCore(str, false, false, false);
}

function escapeProperlyCore(str, bAsUrl)
{
    return escapeProperlyCoreCore(str, bAsUrl, false, false);
}

function escapeUrlForCallback(str)
{
    // Callbacks do not work if a #bookmark is in the URL. If there is a bookmark then we need to remove it. We also need to
    // deal with the scenario where there is not a bookmark but there is an unencoded # as a part of a name/value after the '?'.
    // This is how things should work here:
    //      .../foo.aspx -> .../foo.aspx (unchanged)
    //      .../foo.aspx#bookmark -> .../foo.aspx (bookmark is removed)
    //      .../foo.aspx#bookmark?name=value -> .../foo.aspx?name=value (bookmark is removed)
    //      .../foo.aspx#bookmark?name1=value#extra1&name2=value2 -> .../foo.aspx?name1=value#extra1&name2=value2 (only the bookmark # is removed)
    //      .../foo.aspx?name1=value#extra1&name2=value2 -> .../foo.aspx?name1=value#extra1&name2=value2 (unchanged)
    var iPound = str.indexOf("#");
    var iQues=str.indexOf("?");
    if ((iPound > 0) && ((iQues == -1) || (iPound < iQues)))
    {
        var strNew = str.substr(0, iPound);
        if (iQues > 0)
        {
            strNew += str.substr(iQues); // Need to include the '?' along with the "name=value" pairs.
        }
        str = strNew;
    }
    return escapeProperlyCoreCore(str, true, false, true);
}

function PageUrlValidation(url)
{
	if (url.substr(0, 4) != "http" && url.substr(0,1) != "/")
	{   // This alert should not fire in regular page
		// unless someone is messing up with the page or page URL.
		var L_InvalidPageUrl_Text = "Invalid page URL: ";
		alert(L_InvalidPageUrl_Text + url);
		return "";
	}
	else
		return url;    
}

var g_currentID;
var g_currentShowing = "DatePickerDiv";
var g_strDatePickerFrameID = "DatePickerFrame";
var g_strDatePickerImageID = "DatePickerImage";
var g_scrollLeft;
var g_scrollTop;

function getOffsetTop(elem, value)
{
    if (elem == null) return value;

    if (elem.tagName.toUpperCase() == "TD" && elem.runtimeStyle.borderTopStyle != "none")
    {
        var shift = parseInt(elem.runtimeStyle.borderTopWidth);
        if (!isNaN(shift)) 
        {
            value += shift;
        }
    }
    
    return getOffsetTop(elem.offsetParent, elem.offsetTop - elem.scrollTop + value);
}

function getOffsetLeft(elem, value)
{
    if (elem == null) return value;
    
    if (elem.tagName.toUpperCase() == "TD" && elem.runtimeStyle.borderLeftStyle != "none")
    {
        var shift = parseInt(elem.runtimeStyle.borderLeftWidth);
        if (!isNaN(shift)) {
            value += shift;
        }
    }
    
    return getOffsetLeft(elem.offsetParent, elem.offsetLeft - elem.scrollLeft + value);
}

function getDate(field,serverDate)
{
	if (field.value != null)
		return field.value;
	else
		return serverDate;
}

function PositionFrame(thediv)
{
	var elt = document.getElementById(thediv);
	var ifrm = GetParentWindow(document).frameElement;
	if (ifrm == null || elt == null)
		return;

    if (elt.offsetWidth > 0)    
        ifrm.style.width = elt.offsetWidth + "px";
    if (elt.offsetHeight > 0)
	    ifrm.style.height = elt.offsetHeight + "px";

	var elm = document.getElementById(g_currentID);
	if (elm == null)
		return;
	try { elm.focus(); } catch (exception) {}
	return;
}

function HideUnhide(nhide,nunhide, id, focusElementID)
{
	Hide(nhide);
	Unhide(nunhide);

	PositionFrame(nunhide);
	g_currentID = id;
	g_currentShowing = nunhide;
	var focusElement = document.getElementById(focusElementID);
	if (focusElement != null)
	    focusElement.focus();
}

function datereplace(ourl,pattern,newstr)
{
  var str = new String(ourl);
  var res = str.indexOf(pattern);
  if (res != -1)
  {
	 var resString = str.substring(0,res);
  	 resString += newstr;
	 var resapp = str.indexOf("&",res);
	 if (resapp != -1)
	 {
		resString += str.substr(resapp+1);
	 }		
	return resString;
  }
  else
  {
    var q = str.indexOf("?");
    if (q == -1) str += "?";
    if (str.charAt(str.length-1) != '&') str += "&";
	str += newstr;
	return str;
  }
}
function MoveToDate(dt, isPreviousMonthMove)
{
  var ourl = document.location.href;
  var pattern = "date=";
    
  // remove any previous month move settings
  ourl=datereplace(ourl,"Previous=", "");
  
  if (isPreviousMonthMove)
    ourl = ourl + "&Previous=True";    
    
  if (timePortion != null)
    dt = dt + timePortion;

  // Replace any date elements
  ourl = datereplace(ourl,pattern,"date="+escapeProperly(dt)+"&");
  // reset the location to the new one
  document.location.replace(ourl);
  return true;
}

function GetParentWindow(doc) {
	// try the ie/opera way
	var retval = doc.parentWindow;
	// now the firefox/safari way
	if (!retval)
		retval = doc.defaultView;
	return retval;
}

function OnKeyDown(elem)
{
	var evtSource = GetParentWindow(elem.document).event;
	var nKeyCode = evtSource.keyCode;
	
	switch (nKeyCode)
		{
	case 27: // Escape
		evtSource.returnValue = false;
		ClosePicker();
		break;
		
	case 38: // Up
		evtSource.returnValue = false;
		MoveDays(-7);
		break;

	case 40: // Down
		evtSource.returnValue = false;
		MoveDays(7);
		break;

	case 37: // Left
		evtSource.returnValue = false;
		MoveDays(-1);
		break;

	case 39: // Right
		evtSource.returnValue = false;
		MoveDays(1);
		break;
		}
}

function ClosePicker()
{
	var ifrm = GetParentWindow(document).frameElement;
	if (ifrm == null)
	{
		return;
	}
	ifrm.resultfunc();
	// Hide the picker.
	ifrm.style.display = "none";
	ifrm = null;
}

function MoveDays(iday)
{
	var stNextID;
	// YYYYMMDD
	if (g_currentID == null || g_currentID.length < 6)
		return;

	var yr = g_currentID.substr(0, 4) - 0;
	var mon = g_currentID.substr(4, 2) - 0;
	var day = g_currentID.substr(6, 2) - 0;

	if (day + iday < 1)
	{
		return;
	}
	else
	{
		stNextID = g_currentID.substr(0, 6) + St2Digits(day+iday);
		var elm = document.getElementById(stNextID);
		if (elm == null)
			return;
		g_currentID = stNextID;
		try { elm.focus(); } catch (exception) {}
	}
}

function St2Digits(w)
{
	var st = "";
	if (w < 0)
		return st;
	if (w < 10)
		st += "0";
	st += w;
	return st;
}

function clickDatePicker(field, src, datestr)
{
	var date;
	var objField = document.getElementById(field);
	var fieldid;
	if (event != null)
		event.cancelBubble = true;
	if(field == null && this.Picker != null)
	{
		this.Picker.style.display = "none";
		this.Picker = null;
	}
	else if (objField != null)
	{
		var fieldelm = document.getElementById(field);
		if(fieldelm != null && fieldelm.isDisabled)
			return;
		date = getDate(objField, datestr);
		fieldid = objField.id;
		var objDatePickerImage = document.getElementById(fieldid+g_strDatePickerImageID);
		clickDatePickerHelper(fieldid, fieldid+g_strDatePickerFrameID, objDatePickerImage, date, src, OnSelectDate, OnPickerFinish);
		document.body.onclick = OnPickerFinish;
	}
}

function clickDatePickerHelper(textboxid, iframeid, objImage, datestr, iframesrc, OnSelectDateCallback, onpickerfinishcallback)
{
	var strCurrentResultFieldId = "";
		
	// If date picker is already open, hide it
	if (this.Picker != null)
	{
		this.Picker.style.display = "none";
		strCurrentResultFieldId = this.Picker.resultfield.id;
		if (this.Picker.resultfunc != null)
		{
			this.Picker.resultfunc();
		}
		this.Picker = null;
	}
	
	// 1. Click on the button for currently open datepicker ==> close the open datepicker [i.e. act as toggle button]
	// 2. Click on the button for a different datepicker ==> close the open datepicker and open the clicked one
	if (strCurrentResultFieldId == textboxid)
	{
		// Case 1 - We have already closed the datepicker and we are done
		return;
		
		// Case 2 - Continue to open the clicked datepicker
	}
	
	// open the date picker
	if (textboxid != null)
	{
		// Get the date picker iframe
		this.Picker = document.getElementById(iframeid);
 		if (this.Picker == null)
 			return;

		g_scrollLeft = document.body.scrollLeft;
		g_scrollTop = document.body.scrollTop;
		this.Picker.attachEvent("onreadystatechange", OnIframeLoadFinish);
		// Get the textbox associated with the date picker
 		this.Picker.resultfield = document.getElementById(textboxid);
 		
 		// Set the callback function to be called from the iframe when a date is selected
 		this.Picker.OnSelectDateCallback = OnSelectDateCallback;
 		
 		// Set the callback function to be called from the iframe when the date picker iframe is closed
 		this.Picker.resultfunc = onpickerfinishcallback;
 		
 		// Navigate the iframe to the given iframesrc + (date to select by default)
 		// Date to select is either the date set in the textbox or a default date
 		var strNewPickerSrc = PageUrlValidation(iframesrc) + escapeProperly(datestr);
		this.Picker.src = strNewPickerSrc;
		
		// Position the iframe
		var iframeTop = getOffsetTop(objImage, 1);
		var iframeLeft = getOffsetLeft(objImage, 1);
		var containerTop = getOffsetTop(this.Picker.offsetParent, 1);
		var containerLeft = getOffsetLeft(this.Picker.offsetParent, 1);
		this.Picker.style.pixelTop = iframeTop - containerTop + objImage.offsetHeight + 1;
		this.Picker.style.pixelRight = iframeLeft - containerLeft + objImage.offsetWidth + 1;
		if (this.Picker.currentStyle.direction=="rtl")
		{
			var cx=this.Picker.offsetParent.offsetWidth;
			this.Picker.style.pixelLeft = iframeLeft - containerLeft + objImage.offsetWidth + 1;
			this.Picker.style.pixelLeft=cx - this.Picker.style.pixelLeft;
		}
		else
		{
			this.Picker.style.pixelRight = iframeLeft - containerLeft + objImage.offsetWidth + 1;
		}
	}
}

function ClickDay(date)
{
	var ifrm=GetParentWindow(document).frameElement;
	if (ifrm==null)
	{
		return MoveToDate(date, false);
	}
	
	// Use callback to set date
	OnSelectDate(ifrm.resultfield, date);
	
	// Use callback to close iframe
	var resultfunc=ifrm.resultfunc;
	resultfunc(ifrm.resultfield);
	return true;
}

function OnPickerFinish(resultfield)
{
	clickDatePicker(null,"","");
}

function OnSelectDate(resultfield, date)
{
    if (timePortion != null)
        date = date + timePortion;

	var autoPostBack=resultfield.attributes.getNamedItem("AutoPostBack");
	var shouldPostBack=(autoPostBack!=null && autoPostBack.value=="1" && resultfield.value != date);
	var shouldNotifyChange = (resultfield.value != date);
	resultfield.value=date;
	if (shouldPostBack) window.setTimeout("__doPostBack('"+resultfield.id+"','')",0);
}

function OnIframeLoadFinish(state)
{
	if(this.Picker != null &&
		this.Picker.readyState != null && 
		this.Picker.readyState == "complete")
	{
		document.body.scrollLeft = g_scrollLeft;
		document.body.scrollTop = g_scrollTop;
		this.Picker.style.display = "block";
		document.frames(this.Picker.id).focus();
	}
}

function Hide(nhide)
{
	var eltHide=document.getElementById(nhide);
	if (eltHide !=null)
		eltHide.style.display="none";
}
function Unhide(nunhide)
{
	var eltUnhide=document.getElementById(nunhide);
	if (eltUnhide !=null)
		eltUnhide.style.display="block";
}
function ShowLoading()
{
    Hide(g_currentShowing);
    Unhide('LoadingDiv');
}
