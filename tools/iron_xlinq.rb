require "System.Xml.Linq"
include System::Xml::Linq

class XElement
  def elements(name)
    Elements XName.get(name)
  end
  def attribute(name)
    attr = Attribute(XName.get(name))
    if(attr != nil)
      attr.value.to_s
    else
      nil
    end
  end
end